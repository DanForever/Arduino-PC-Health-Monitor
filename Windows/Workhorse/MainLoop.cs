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

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HardwareMonitor
{
	public class Main
	{
		#region Private fields

		// Configs
		private Monitor.Config.Computer _sensorConfig;
		private Plugin.Config.Config _pluginConfig;
		private Icon.Config _iconConfig;

		// Plugins
		private Plugin.Manager _pluginManager;

		// Connected devices
		private List<Device> _devices = new List<Device>();

		private bool _OSInSleepState = false;

		private Layout.LayoutManager _layoutManager = new Layout.LayoutManager();

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// The "framerate" or total time from one hardware poll to the next.
		/// Dictates how long the thread sleeps for between polls
		/// </summary>
		public int UpdateRate { get; set; } = 1000;

		/// <summary>
		/// Set this to true when you want the application to end
		/// </summary>
		public bool RequestExit { get; set; } = false;

		/// <summary>
		/// The frontend (i.e. platform specific) project needs to monitor the OS sleep state
		/// and update this boolean accordingly, to ensure that the devices sleep as well
		/// </summary>
		public bool OSInSleepState
		{
			get { return _OSInSleepState; }
			set
			{
				_OSInSleepState = value;
				if(_OSInSleepState)
				{
					// Disconnect all devices
					foreach (Device device in Devices)
					{
						device.Connection.Disconnect();
					}
				}
			}
		}

		public IEnumerable<Device> Devices => _devices;

		public IEnumerable<Plugin.ISource> Sources => _pluginManager.Sources;

		#endregion Public Properties

		#region Events

		public delegate void FeedbackHandler(string text);
		public event FeedbackHandler Feedback;

		public delegate void DeviceConnectionHandler(Device device);
		public event DeviceConnectionHandler DeviceConnected;
		public event DeviceConnectionHandler DeviceDisconnected;

		#endregion Events

		#region Public Methods

		/// <summary>
		/// Call this to start the application running
		/// </summary>
		/// <returns>Program return code</returns>
		public async Task<int> Run()
		{
			Initialize();

			Stopwatch stopWatch = new();

			while(!RequestExit)
			{
				// Time how long the core loop takes
				stopWatch.Restart();

				// Core Loop
				UpdateConnections();
				await UpdateDevices();

				// Core loop has finished
				stopWatch.Stop();

				// Sleep until the next update
				int timeTakenToUpdate = (int)stopWatch.ElapsedMilliseconds;

				if (timeTakenToUpdate < UpdateRate)
				{
					int timeToSleep = UpdateRate - timeTakenToUpdate;
					await Task.Delay(timeToSleep);
				}
			}

			return 0;
		}

		#endregion Public Methods

		#region Private Methods

		private async void Initialize()
		{
			_sensorConfig = Monitor.Config.Computer.Load("Data/sensors.xml");
			_pluginConfig = Plugin.Config.Config.Load("Data/plugins.xml");
			_iconConfig = Icon.Config.Load("Data/icons.xml");

			_pluginManager = new Plugin.Manager(_pluginConfig);

			_layoutManager.Load();

			await Releases.Releases.UpdateLatestReleases();
		}

		private bool IsAlreadyConnected(Connection.AvailableConnection connection)
		{
			foreach (Device device in _devices)
			{
				if (device.Connection.Name == connection.Name)
					return true;
			}

			return false;
		}

		private bool ShouldRemoveDevice(Device device)
		{
			if (!device.IsConnected)
			{
				Feedback?.Invoke($"Device disconnected: {device.Connection.Name}");
				DeviceDisconnected?.Invoke(device);
				return true;
			}

			return false;
		}

		private void UpdateConnections()
		{
			// First remove any connections that are no longer valid
			_devices.RemoveAll(device => ShouldRemoveDevice(device));

			// If the OS is asleep, don't continue any further
			if (OSInSleepState)
				return;

			// Now check for any new connections we can make
			var availableConnections = Connection.Connections.Enumerate();

			foreach (Connection.AvailableConnection availableConnection in availableConnections)
			{
				// This is potentially n^2, but I'm not expecting the list being checked to be more than maybe 2 items
				if (IsAlreadyConnected(availableConnection))
					continue;

				Connection.ActiveConnection activeConnection = null;

				try
				{
					activeConnection = availableConnection.Connect();
				}
				catch (Connection.ConnectionFailedException e)
				{
					Feedback?.Invoke($"Failed to connect to {availableConnection.Name}: {e.Message}");
				}

				if (activeConnection != null && activeConnection.IsOpen)
				{
					Device device = new Device();
					device.IdentityRecieved += (Device device) => PrintDeviceIdentity(device);

					device.Icons = _iconConfig;
					device.Connection = activeConnection;

					Feedback?.Invoke($"Connected to {device.Connection.Name}");
					DeviceConnected?.Invoke(device);

					_devices.Add(device);
				}
			}
		}

		private void PrintDeviceIdentity(Device device)
		{
			string[] MicrocontrollerStrings = new string[]
			{
				"Teensy 3.2",
				"Teensy 4.0",
				"Seeeduino Xiao",
			};

			string[] ScreenStrings = new string[]
			{
				"ILI9341",
				"ILI9486",
				"ILI9488",
				"NT35510",
			};

			string[] ResolutionStrings = new string[]
			{
				"240 x 320",
				"320 x 480",
				"480 x 800",
			};

			Feedback?.Invoke($"Device {device.Connection.Name} identity:");
			Feedback?.Invoke($"  - Microcontroller:   {MicrocontrollerStrings[(int)device.Microcontroller]}");
			Feedback?.Invoke($"  - Screen controller: {ScreenStrings[(int)device.Screen]}");
			Feedback?.Invoke($"  - Screen Resolution: {ResolutionStrings[(int)device.Resolution]}");
		}

		private async Task UpdateDevices()
		{
			Monitor.Snapshot snapshot = await Monitor.Asynchronous.Watcher.Poll(_sensorConfig, _pluginManager.Sources);

			foreach (Device device in _devices)
			{
				if (!device.IsConnected)
					continue;

				if(device.Layout == null)
				{
					Layout.Config layout = _layoutManager.GetLayout(device.Resolution, device.Orientation);
					if (layout != null)
						await device.SetLayout(layout);
					else
						continue;
				}

				await device.Update(snapshot);
			}
		}

		#endregion Private Methods
	}
}
