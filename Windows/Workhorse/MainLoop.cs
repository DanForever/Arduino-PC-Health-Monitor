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
using System.Threading;
using System.Threading.Tasks;

namespace HardwareMonitor
{
	public class Main
	{
		/// <summary>
		/// The "framerate" or total time from one hardware poll to the next.
		/// Dictates how long the thread sleeps for between polls
		/// </summary>
		public int UpdateRate { get; set; } = 1000;

		/// <summary>
		/// Set this to true when you want the application to end
		/// </summary>
		public bool RequestExit { get; set; } = false;

		public delegate void FeedbackHandler(string text);
		public event FeedbackHandler Feedback;

		private Monitor.Config.Computer _sensorConfig;
		private Protocol.Config _protocolConfig;
		private Plugin.Config.Config _pluginConfig;
		private Icon.Config _iconConfig;

		private Plugin.Manager _pluginManager;

		// @todo: Refactor so that the connection is done outside the Device class and then passed to a new device on success
		private Device _temp;

		private List<Device> _devices = new List<Device>();

		/// <summary>
		/// Call this to start the application running
		/// </summary>
		/// <returns>Program return code</returns>
		public async Task<int> Run()
		{
			Initialize();

			Stopwatch stopWatch = new Stopwatch();

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
				if(timeTakenToUpdate < UpdateRate)
				{
					int timeToSleep = UpdateRate - timeTakenToUpdate;
					Thread.Sleep(timeToSleep);
				}
			}

			return 0;
		}

		private void Initialize()
		{
			_sensorConfig = Monitor.Config.Computer.Load("Data/sensors.xml");
			_protocolConfig = Protocol.Config.Load("Data/protocol.xml");
			_pluginConfig = Plugin.Config.Config.Load("Data/plugins.xml");
			_iconConfig = Icon.Config.Load("Data/icons.xml");

			_pluginManager = new Plugin.Manager(_pluginConfig);
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
				return true;
			}

			return false;
		}

		private void UpdateConnections()
		{
			// First remove any connections that are no longer valid
			_devices.RemoveAll(device => ShouldRemoveDevice(device));

			// Now check for any new connections we can make
			var availableConnections = Connection.Manager.EnumerateAvailableConnections();

			foreach (Connection.AvailableConnection connection in availableConnections)
			{
				// This is potentially n^2, but I'm not expecting the list being checked to be more than maybe 2 items
				if (IsAlreadyConnected(connection))
					continue;

				if (_temp == null)
				{
					_temp = new Device();
					_temp.Protocol = _protocolConfig;
					_temp.Icons = _iconConfig;
				}

				_temp.Connect(connection);

				if (_temp.IsConnected)
				{
					Feedback?.Invoke($"Connected to {_temp.Connection.Name}");

					_devices.Add(_temp);
					_temp = null;
				}
			}
		}

		private async Task UpdateDevices()
		{
			foreach (Device device in _devices)
			{
				if (!device.IsConnected)
					continue;

				Monitor.Snapshot snapshot = await Monitor.Asynchronous.Watcher.Poll(_sensorConfig, _pluginManager.Sources);

				await device.Update(snapshot);
			}
		}
	}
}
