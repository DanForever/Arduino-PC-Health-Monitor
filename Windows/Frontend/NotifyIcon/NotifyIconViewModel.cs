/*
 *   Arduino PC Health Monitor (PC Companion app)
 *   Polls the hardware sensors for data and forwards them on to the arduino device
 *   Copyright (C) 2021 Daniel Neve
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Reg = Microsoft.Win32;
using TS = Microsoft.Win32.TaskScheduler;

namespace HardwareMonitor.NotifyIcon
{
	/// <summary>
	/// This is an example of how to use the registry to launch the application on windows startup.
	/// It's probably the simplest and easiest solution, but unfortunately does not work for us because we need admin privilages in order to gather all the metrics
	/// </summary>
	static class Registry
	{
		#region Private consts

		private static readonly string RunOnStartupRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
		private static readonly string ApplicationRegistryName = "DanHardwareMonitor";

		#endregion Private consts

		#region Public Methods

		public static void ToggleRunOnStartup()
		{
			Reg.RegistryKey rk = Reg.Registry.CurrentUser.OpenSubKey(RunOnStartupRegistryKey, true);

			if (HasRunOnStartupRegistryValue())
				rk.DeleteValue(ApplicationRegistryName, false);
			else
			{
				string assemblyPath = System.Reflection.Assembly.GetEntryAssembly().Location;

				// Quick and dirty hack to deal with c# not quite giving us the correct path for some reason...
				if (System.IO.Path.GetExtension(assemblyPath) == ".dll")
					assemblyPath = System.IO.Path.ChangeExtension(assemblyPath, ".exe");

				//rk.SetValue(ApplicationRegistryName, typeof(NotifyIconViewModel).Assembly.Location);
				rk.SetValue(ApplicationRegistryName, assemblyPath);
			}
		}

		public static bool HasRunOnStartupRegistryValue()
		{
			Reg.RegistryKey rk = Reg.Registry.CurrentUser.OpenSubKey(RunOnStartupRegistryKey, false);

			return rk.GetValue("DanHardwareMonitor") != null;
		}

		#endregion Public Methods
	}

	/// <summary>
	/// This method uses the windows task scheduler to launch an application on windows startup
	/// </summary>
	static class TaskScheduler
	{
		#region Private consts

		private static readonly string TaskName = "RunDanHardwareMonitorAtLogon";

		#endregion Private consts

		#region Public Methods

		public static void ToggleRunOnStartup()
		{
			if(TaskExists())
			{
				DeleteTask();
				return;
			}

			string assemblyPath = System.Reflection.Assembly.GetEntryAssembly().Location;

			// Quick and dirty hack to deal with c# not quite giving us the correct path for some reason...
			if (System.IO.Path.GetExtension(assemblyPath) == ".dll")
				assemblyPath = System.IO.Path.ChangeExtension(assemblyPath, ".exe");

			CreateTask(assemblyPath);
		}

		public static bool TaskExists()
		{
			return TS.TaskService.Instance.GetTask(TaskName) != null;
		}

		#endregion Public Methods

		#region Private Methods

		private static void CreateTask(string assemblyPath)
		{
			// Create a new task definition and assign properties
			TS.TaskDefinition td = TS.TaskService.Instance.NewTask();
			td.RegistrationInfo.Description = "Run Dan's hardware monitor at logon";
			td.Principal.LogonType = TS.TaskLogonType.InteractiveToken;
			td.Principal.RunLevel = TS.TaskRunLevel.Highest;

			TS.LogonTrigger lt = new();
			lt.UserId = Environment.UserName;
			td.Triggers.Add(lt);

			// Add an action that will launch Notepad whenever the trigger fires
			td.Actions.Add(new TS.ExecAction(assemblyPath, workingDirectory: Path.GetDirectoryName(assemblyPath)));

			// Register the task in the root folder
			TS.TaskService.Instance.RootFolder.RegisterTaskDefinition(TaskName, td);
		}

		private static void DeleteTask()
		{
			TS.TaskService.Instance.RootFolder.DeleteTask(TaskName);
		}

		#endregion Private Methods
	}

	class NotifyIconViewModel
	{
		#region Private Fields

		private ICommand _exitApplicationCommand = new DelegateCommand { CommandAction = () => { ((App)Application.Current).Program.RequestExit = true; } };
		private ICommand _toggleRunOnStartupCommand = new DelegateCommand { CommandAction = TaskScheduler.ToggleRunOnStartup };

		#endregion Private Fields

		#region Public Properties

		public ICommand ExitApplicationCommand => _exitApplicationCommand;
		public ICommand ToggleRunOnStartupCommand => _toggleRunOnStartupCommand;
		public bool WillRunOnStartup => TaskScheduler.TaskExists();

		#endregion Public Properties
	}

	public class DelegateCommand : ICommand
	{
		#region Public Properties

		public Action CommandAction { get; set; }
		public Func<bool> CanExecuteFunc { get; set; }

		#endregion Public Properties

		#region Public Methods

		public void Execute(object parameter)
		{
			CommandAction();
		}

		public bool CanExecute(object parameter)
		{
			return CanExecuteFunc == null || CanExecuteFunc();
		}

		#endregion Public Methods

		#region Events

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		
		#endregion Events
	}
}
