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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareMonitor.Connection
{
	public abstract class Packet
    {
        public static byte[] MessageHeader => Encoding.ASCII.GetBytes("dan<`");
        public static byte[] MessageFooter => Encoding.ASCII.GetBytes("`>dan");

        public IList<ActiveConnection> Connections { get; set; }

        protected byte[] SerializeData(params dynamic[] args)
        {
            MemoryStream stream = new MemoryStream();

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(MessageHeader);

                foreach (var arg in args)
                {
                    if (arg.GetType().IsEnum)
                        writer.Write((byte)arg);
                    else if (arg.GetType() == typeof(string))
                        writer.Write(Encoding.ASCII.GetBytes(arg));
                    else
                        writer.Write(arg);
                }

                writer.Write(MessageFooter);

                return stream.ToArray();
            }
        }

		public abstract void Send(params dynamic[] args);

	}

	public class SimplePacket : Packet
    {
        public override void Send(params dynamic[] args)
        {
            byte[] data = SerializeData(args);

            foreach(ActiveConnection connection in Connections)
            {
                if (connection.IsOpen)
                {
                    connection.Send(data);
                }
            }
        }
    }

    public class GuaranteedPacket : Packet
    {
        private static int TimeOut = 3000;
        private static ushort _idPool = 0;

        private ushort _id = ++_idPool;
        private byte[] _data;

        public ushort Id => _id;

		public override async void Send(params dynamic[] args)
		{
			await SendAsync(args);
		}

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

            Debug.Print($"Acknowledgement of packet {Id} received by all connections");
        }

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
    }
}
