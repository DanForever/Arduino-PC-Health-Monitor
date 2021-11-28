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
using System.Threading.Tasks;

namespace HardwareMonitor.Testing
{
	class Program
	{
		static async Task<int> Main(string[] args)
		{
			var sensorConfig = Monitor.Config.Computer.Load("Data/sensors.xml");
			var protocolConfig = Protocol.Config.Load("Data/protocol.xml");
			var pluginConfig = Plugin.Config.Config.Load("Data/plugins.xml");
			var iconConfig = Icon.Config.Load("Data/icons.xml");

			var pluginManager = new Plugin.Manager(pluginConfig);

			Console.WriteLine("Waiting for device...");
			Device device = new Device();
			device.Protocol = protocolConfig;
			device.Icons = iconConfig;

			while(!device.IsConnected)
			{
				var availableConnections = Connection.Manager.EnumerateAvailableConnections();

				foreach(Connection.AvailableConnection connection in availableConnections)
				{
					device.Connect(connection);

					if(device.IsConnected)
						break;
				}
			}

			Console.WriteLine("Connected!");
			//HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.CpuIcon, "images/ryzen_black.bmp", device);
			//HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.GpuIcon, "images/nvidia_rtx.bmp", device);
			//HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.MemoryIcon, "images/gskill_tzn.bmp", device);

			while (device.IsConnected)
			{
				Monitor.Snapshot snapshot = await Monitor.Asynchronous.Watcher.Poll(sensorConfig, pluginManager.Sources);

				await device.Update(snapshot);
			}

			return 0;
		}
    }
}
