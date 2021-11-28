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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace HardwareMonitor.Testing
{
	class Program
	{
		static void DebugSendIcon(Protocol.Metrics metric, Device device, string filepath)
		{
			Stream imageStreamSource = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
	
			BmpBitmapDecoder decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			BitmapSource bitmapSource = decoder.Frames[0];
	
			if (bitmapSource.Format != System.Windows.Media.PixelFormats.Bgr565)
			{
			    Debug.Print($"Image at path `{filepath}` is not RGB565!");
			    return;
			}
	
			byte[] icon = new byte[bitmapSource.PixelHeight * bitmapSource.PixelWidth * sizeof(short)];
	
			bitmapSource.CopyPixels(icon, bitmapSource.PixelWidth * sizeof(short), 0);

			Connection.IconSender.DebugSend(bitmapSource.PixelWidth, bitmapSource.PixelHeight, icon, metric, device);
		}

		static async Task<int> Main(string[] args)
		{
			var sensorConfig = Monitor.Config.Computer.Load("Data/sensors.xml");
			var protocolConfig = Protocol.Config.Load("Data/protocol.xml");
			var pluginConfig = Plugin.Config.Config.Load("Data/plugins.xml");

			var pluginManager = new Plugin.Manager(pluginConfig);

			Console.WriteLine("Waiting for device...");
			Device device = new Device();
			device.Protocol = protocolConfig;
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
			HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.CpuIcon, "images/ryzen_black.bmp", device);
			HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.GpuIcon, "images/nvidia_rtx.bmp", device);
			HardwareMonitor.Connection.IconSender.Send(Protocol.Metrics.MemoryIcon, "images/gskill_tzn.bmp", device);

			while (device.IsConnected)
			{
				Monitor.Snapshot snapshot = await Monitor.Asynchronous.Watcher.Poll(sensorConfig, pluginManager.Sources);

				device.Update(snapshot);
			}

			return 0;
		}
    }
}
