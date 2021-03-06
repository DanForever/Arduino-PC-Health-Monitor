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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using HardwareMonitor;
using HardwareMonitor.Plugin;

using Microsoft.Diagnostics.Tracing.Session;

namespace FramerateMetrics
{
	public class MillisecondRecord
	{
		#region Private Fields

		private double _previousTimestamp;
		private double _currentTimestamp;

		#endregion Private Fields

		#region Public Properties

		public string ProcessName { get; set; }
		public string DisplayName { get; set; }
		public double PreviousCounter => _previousTimestamp;
		public double Current
		{
			get { return _currentTimestamp; }
			set
			{
				_previousTimestamp = _currentTimestamp;
				_currentTimestamp = value;
			}
		}
		
		#endregion Public Properties
	}

	public class Framerate : ISensor
	{
		#region ISensor

		public string Name { get; set; }

		public float Value { get; set; }

		public SensorType Type => SensorType.Data;

		public IHardware Hardware { get; set; }
		
		#endregion ISensor
	}

	public class Program : IHardware
	{
		#region IHardware

		public string Name { get; set; }

		public HardwareType Type => HardwareType.Software;

		public IEnumerable<ISensor> Sensors => Framerates;

		#endregion IHardware

		#region Public Properties

		public Framerate[] Framerates { get; set; } = new Framerate[2] { new Framerate() { Name = "fms" }, new Framerate() { Name = "fps" } };

		#endregion Public Properties

		#region C-Tor

		public Program()
		{
			foreach(Framerate framerate in Framerates)
			{
				framerate.Hardware = this;
			}
		}

		#endregion C-Tor
	}

	public class Source : ISource
	{
		#region Private Fields

		private Program[] _programs = new Program[1] { new Program() };

		private Config.Config _config;

		TraceEventSession _etwSession;
		Thread _etwThread;
		object _sync = new object();
		Dictionary<int, MillisecondRecord> _millisecondRecords = new Dictionary<int, MillisecondRecord>();

		#endregion Private Fields

		#region Public Constants

		//event codes (https://github.com/GameTechDev/PresentMon/blob/40ee99f437bc1061a27a2fc16a8993ee8ce4ebb5/PresentData/PresentMonTraceConsumer.cpp)
		public const int EventID_D3D9PresentStart = 1;
		public const int EventID_DxgiPresentStart = 42;

		//ETW provider codes
		public static readonly Guid DXGI_provider = Guid.Parse("{CA11C036-0102-4A2D-A6AD-F03CFED5D3C9}");
		public static readonly Guid D3D9_provider = Guid.Parse("{783ACA0A-790E-4D7F-8451-AA850511C6B9}");

		#endregion Public Constants

		#region C-Tor

		public Source()
		{
			_config = Config.Config.Load(Path.Join("Data", "frameratemetrics.xml"));

			//create ETW session and register providers
			_etwSession = new TraceEventSession("mysess");
			_etwSession.StopOnDispose = true;
			_etwSession.EnableProvider("Microsoft-Windows-D3D9");
			_etwSession.EnableProvider("Microsoft-Windows-DXGI");

			//handle event
			_etwSession.Source.AllEvents += data =>
			{
				//filter out frame presentation events
				if (((int)data.ID == EventID_D3D9PresentStart && data.ProviderGuid == D3D9_provider) ||
				((int)data.ID == EventID_DxgiPresentStart && data.ProviderGuid == DXGI_provider))
				{
					int pid = data.ProcessID;

					// Find out if we've already grabbed the metadata for this process
					bool processDataCaptured = false;
					lock (_sync)
					{
						processDataCaptured = _millisecondRecords.ContainsKey(pid);
					}

					// If we haven't...
					if (!processDataCaptured)
					{
						// Grab the process
						Process process = TryGetProcessById(pid);
						if (process == null || process.HasExited)
							return;

						// We need to know when the process closes so we can stop displaying it on the screen
						process.EnableRaisingEvents = true;
						process.Exited += Process_Exited;

						// Use the window title first if possible, then the process name, and then finally the process id if there's nothing else
						string processName = process.ProcessName;
						string displayName = process.MainWindowTitle;

						if (string.IsNullOrWhiteSpace(displayName))
							displayName = process.ProcessName;

						if (string.IsNullOrWhiteSpace(displayName))
							displayName = pid.ToString();

						if (string.IsNullOrWhiteSpace(processName))
							processName = pid.ToString();

						// Then store all that metadata in the record
						lock (_sync)
						{
							_millisecondRecords[pid] = new MillisecondRecord();
							_millisecondRecords[pid].Current = data.TimeStampRelativeMSec;
							_millisecondRecords[pid].DisplayName = displayName;
							_millisecondRecords[pid].ProcessName = processName;
						}
					}

					// Finally, record the frame time in the record
					lock (_sync)
					{
						_millisecondRecords[pid].Current = data.TimeStampRelativeMSec;
					}
				}
			};

			_etwThread = new Thread(()=> _etwSession.Source.Process());
			_etwThread.IsBackground = true;
			_etwThread.Start();
		}

		#endregion C-Tor

		#region Private Methods

		static Process TryGetProcessById(int pid)
		{
			try
			{
				return Process.GetProcessById(pid);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
		
		#endregion Private Methods

		#region Event Handlers

		private void Process_Exited(object sender, EventArgs e)
		{
			Process process = sender as Process;
			lock(_sync)
			{
				_millisecondRecords.Remove(process.Id);
			}

			process.Exited -= Process_Exited;
			process.Dispose();
		}

		#endregion Event Handlers

		#region ISource

		IEnumerable<IHardware> ISource.Hardware => _programs;

		void ISource.PollingStarted(HardwareMonitor.Monitor.Config.Computer config)
		{
			// First let's reset the output values, incase the game has shut down in the mean time
			Program program = _programs[0];
			program.Name = "No game detected";
			program.Framerates[0].Value = 0;
			program.Framerates[1].Value = 0;

			lock (_sync)
			{
				foreach(var record in _millisecondRecords.Values)
				{
					if(_config.Blacklist.Any(program => program.ProcessNameRegex.IsMatch(record.ProcessName)))
						continue;

					double ms = record.Current - record.PreviousCounter;
					double fps = 1000 / ms;

					program.Name = record.DisplayName;
					program.Framerates[0].Value = (float)ms;
					program.Framerates[1].Value = (float)fps;

					break;
				}
			}
		}

		#endregion ISource
	}
}
