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
using System.IO;
using System.Threading.Tasks;

namespace HardwareMonitor
{
	public class Device
	{
		#region Private Fields

		private bool _onceOnlyDataSent = false;

		#endregion Private Fields

		#region Public Properties

		public Connection.ActiveConnection Connection { get; set; }
		public Protocol.Config Protocol { get; set; }
		public Icon.Config Icons { get; set; }
		public bool IsConnected => Connection is not null && Connection.IsOpen;

		#endregion Public Properties

		#region Public Methods

		public async Task Update(Monitor.Snapshot snapshot)
		{
			if (Protocol == null)
				return;

			if (Connection == null)
				return;

			if (!Connection.IsOpen)
				return;

			foreach(var module in Protocol.Modules)
			{
				await UpdateModule(snapshot, module);
			}

			_onceOnlyDataSent = true;
		}

		#endregion Public Methods

		#region Private Methods

		private async Task UpdateModule(Monitor.Snapshot snapshot, Protocol.Module module)
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
						Protocol.Icon icon = metric as Protocol.Icon;

						if (icon != null && Icons != null)
						{
							string iconPath = Path.Join("Images", $"{Icons.GetIcon(capturedValue.Value)}.bmp");
							HardwareMonitor.Connection.IconSender.Send(icon.Type, iconPath, Connection);
						}
						else
						{
							Connection.GuaranteedPacket packet = new Connection.GuaranteedPacket();
							packet.Connections = new List<Connection.ActiveConnection>() { Connection };
							await packet.SendAsync(HardwareMonitor.Protocol.PacketType.SensorUpdate, metric.Type, capturedValue.Value);
						}
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

		#endregion Private Methods
	}
}
