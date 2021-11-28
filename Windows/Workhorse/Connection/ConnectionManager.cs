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
using System.Linq;

namespace HardwareMonitor.Connection
{
    public class Manager
	{
		private List<ActiveConnection> _activeConnections = new List<ActiveConnection>();

		public bool IsConnected => VerifyConnections();

		public static IEnumerable<AvailableConnection> EnumerateAvailableConnections()
        {
            foreach (AvailableConnection availableConnection in Serial.EnumeratePorts())
            {
                yield return availableConnection;
            }
        }

        public delegate void NewActiveConnectionHandler(ActiveConnection connection);
        public event NewActiveConnectionHandler NewActiveConnection;

        public void Connect(AvailableConnection availableConnection)
        {
            AvailableSerialConnection asc = availableConnection as AvailableSerialConnection;
            if(asc is not null)
            {
                ActiveSerialConnection activeSerialConnection = new ActiveSerialConnection(asc);
                _activeConnections.Add(activeSerialConnection);

                NewActiveConnection?.Invoke(activeSerialConnection);
            } 
        }

        public void Send(params dynamic[] args)
        {
            SimplePacket packet = new SimplePacket();
            packet.Connections = _activeConnections.ToArray();
            packet.Send(args);
        }

        public async void SendGuaranteed(params dynamic[] args)
        {
            GuaranteedPacket packet = new GuaranteedPacket();
            packet.Connections = _activeConnections.ToList();
            await packet.SendAsync(args);
        }

        public GuaranteedPacket SendGuaranteed()
        {
            GuaranteedPacket packet = new GuaranteedPacket();
            packet.Connections = _activeConnections.ToList();

            return packet;
        }

		private bool VerifyConnections()
		{
			_activeConnections.RemoveAll(connection => !connection.IsOpen);

			return _activeConnections.Count > 0;
		}
    }
}
