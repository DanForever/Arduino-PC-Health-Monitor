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

using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.Win32;

namespace HardwareMonitor
{
	public partial class App : Application
	{
		#region Private Fields

		public static App Application => (App)Current;

		private TaskbarIcon _tb;

		#endregion Private Fields

		#region Public Properties

		public Main Program { get; set; }

		#endregion Public Properties

		#region Event Handlers

		private async void Application_Startup(object sender, StartupEventArgs e)
		{
			// Instanticate the class that will perform the main body of work for us
			Program = new Main();

			//initialize NotifyIcon
			_tb = (TaskbarIcon)FindResource("MyNotifyIcon");

			SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

			// Run the application
			int retcode = await Program.Run();

			Shutdown(retcode);
		}

		private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			switch(e.Mode)
			{
			case PowerModes.Suspend:
				Program.OSInSleepState = true;
				break;

			case PowerModes.Resume:
				Program.OSInSleepState = false;
				break;
			}
		}

		#endregion Event Handlers
	}
}
