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

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareMonitor.Connection
{
	/// <summary>
	/// This packet will send the data and await an acknowledgement of receipt. If it is not receieved in a timely manner, the data will be resent
	/// </summary>
	internal class GuaranteedPacket : Packet
    {
		#region private Fields

		private static int TimeOut = 3000;
        private static ushort _idPool = 0;

        private ushort _id = ++_idPool;
        private byte[] _data;

		#endregion private Fields

		#region Public Properties

		public ushort Id => _id;

		#endregion Public Properties

		#region Packet

		public override async void Send(params dynamic[] args)
		{
			await SendAsync(args);
		}

		#endregion Packet

		#region Public Methods

		public async Task SendAsync(params dynamic[] args)
        {
            dynamic[] wrappedArgs = new dynamic[args.Length + 2];
            wrappedArgs[0] = Protocol.PacketType.Guaranteed;
            wrappedArgs[1] = Id;
            args.CopyTo(wrappedArgs, 2);

            _data = SerializeData(wrappedArgs);

            foreach (ActiveConnection connection in Connections)
            {
                if (connection.IsOpen)
                {
                    connection.DataRecieved += Connection_DataRecieved;
                }
            }

            while (Connections.Any())
            {
                foreach (ActiveConnection connection in Connections)
                {
                    connection.Send(_data);
                }

                await Task.Delay(TimeOut);
            }

            //Debug.Print($"Acknowledgement of packet {Id} received by all connections");
        }

		#endregion Public Methods

		#region Event Handlers

		private void Connection_DataRecieved(ActiveConnection connection, byte[] data, int dataLength)
        {
            MemoryStream stream = new MemoryStream(data);

            using (BinaryReader reader = new BinaryReader(stream))
            {
				Protocol.PacketType messageCategory = (Protocol.PacketType)reader.ReadByte();
                ushort id = reader.ReadUInt16();

                if( messageCategory == Protocol.PacketType.GuaranteedAck && id == Id)
                {
                    connection.DataRecieved -= Connection_DataRecieved;

                    Connections.Remove(connection);
                }
            }
        }

		#endregion Event Handlers
	}
}
