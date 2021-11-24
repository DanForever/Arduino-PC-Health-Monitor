using System.Collections.Generic;

namespace HardwareMonitor
{
	public class Device
	{
		private bool _onceOnlyDataSent = false;
		public Connection.ActiveConnection Connection { get; set; }
		public Protocol.Config Protocol { get; set; }
		public bool IsConnected => Connection is not null && Connection.IsOpen;

		public void Connect(Connection.AvailableConnection availableConnection)
		{
			try
			{
				Connection = availableConnection.Connect();
			}
			catch(System.IO.FileNotFoundException)
			{
			}
		}

		public void Update(Monitor.Snapshot snapshot)
		{
			if (Protocol == null)
				return;

			if (Connection == null)
				return;

			if (!Connection.IsOpen)
				return;

			foreach(var module in Protocol.Modules)
			{
				UpdateModule(snapshot, module);
			}

			_onceOnlyDataSent = true;
		}

		private async void UpdateModule(Monitor.Snapshot snapshot, Protocol.Module module)
		{
			foreach(var metric in module.Metrics)
			{
				if (metric.NoUpdate && _onceOnlyDataSent)
					continue;

				Monitor.Capture capturedValue;
				if (!string.IsNullOrWhiteSpace(metric.Capture) && snapshot.Captures.TryGetValue(metric.Capture, out capturedValue))
				{
					if(metric.NoUpdate)
					{
						Connection.GuaranteedPacket packet = new Connection.GuaranteedPacket();
						packet.Connections = new List<Connection.ActiveConnection>() { Connection };
						await packet.SendAsync(HardwareMonitor.Protocol.PacketType.SensorUpdate, metric.Type, capturedValue.Value);
					}
					else
					{
						Connection.SimplePacket packet = new Connection.SimplePacket();
						packet.Connections = new List<Connection.ActiveConnection>() { Connection };
						packet.Send(HardwareMonitor.Protocol.PacketType.SensorUpdate, metric.Type, capturedValue.Value);
					}
				}
			}
		}
	}
}
