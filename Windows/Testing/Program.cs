using System;
using System.Collections.Generic;
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
			Protocol.Config.SaveDummy();
			Monitor.Config.Computer.SaveDummy();
			var sensorConfig = Monitor.Config.Computer.Load("Data/sensors.xml");
			var protocolConfig = Protocol.Config.Load("Data/protocol.xml");

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
				Monitor.Snapshot snapshot = await Monitor.Asynchronous.Watcher.Poll(sensorConfig);

				device.Update(snapshot);
			}

			return 0;
		}
    }
}
