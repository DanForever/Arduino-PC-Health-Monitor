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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HardwareMonitor.Connection
{
	#region Internal Utility Types

	namespace Bitmap
	{
		/// <summary>
		/// This is the main header or "preamble" containing general bmp file details
		/// </summary>
		internal class Header
		{
			#region Public Properties

			public ushort Signature { get; set; }
			public uint FileSize { get; set; }
			public uint Reserved { get; set; }
			public uint DataOffset { get; set; }

			#endregion Public Properties
		}

		/// <summary>
		/// This is the interesting header that contains the information we need about the bitmap image itself
		/// </summary>
		internal class InfoHeader
		{
			#region Public Properties

			public uint Size { get; set; }
			public uint Width { get; set; }
			public uint Height { get; set; }
			public ushort Planes { get; set; }
			public uint BitCount { get; set; }
			public uint Compression { get; set; }
			public uint ImageSize { get; set; }
			public uint XPixelsPerMetre { get; set; }
			public uint YPixelsPerMetre { get; set; }
			public uint ColoursUsed { get; set; }
			public uint ColoursImportant { get; set; }

			#endregion Public Properties
		}
	}

	internal class Icon
	{
		#region Public Properties

		public string Filepath { get; set; }
		public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte[] Data { get; set; }

		#endregion Public Properties
	}

	#endregion Internal Utility Types

	public static class IconSender
	{
		#region Public Methods

		/// <summary>
		/// Send an icon to a device.
		/// </summary>
		/// <remarks>
		/// It's async void so you can "fire and forget" - there may be a better way to accomplish this - .ConfigureAwait(false) ?
		/// </remarks>
		/// <param name="metric"></param>
		/// <param name="filepath"></param>
		/// <param name="connection"></param>
		public static async void Send(Layout.MappedComponent mappedComponent, string filepath, ActiveConnection connection)
        {
			if (filepath == null)
				return;

            Icon icon = await Load(filepath);

            if (icon == null)
                return;

            await SendChunks(icon, mappedComponent, connection);
        }

		#endregion Public Methods

		#region Private Methods

		private static async Task<Icon> Load(string filepath)
		{
			return await Task.Run(() => LoadSync(filepath));
		}

        private static Icon LoadSync(string filepath)
        {
			using (FileStream stream = File.OpenRead(filepath))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					var header = new Bitmap.Header()
					{
						Signature = reader.ReadUInt16(),
						FileSize = reader.ReadUInt32(),
						Reserved = reader.ReadUInt32(),
						DataOffset = reader.ReadUInt32()
					};

					var infoHeader = new Bitmap.InfoHeader()
					{
						Size = reader.ReadUInt32(),
						Width = reader.ReadUInt32(),
						Height = reader.ReadUInt32(),
						Planes = reader.ReadUInt16(),
						BitCount = reader.ReadUInt16(),
						Compression = reader.ReadUInt32(),
						ImageSize = reader.ReadUInt32(),
						XPixelsPerMetre = reader.ReadUInt32(),
						YPixelsPerMetre = reader.ReadUInt32(),
						ColoursUsed = reader.ReadUInt32(),
						ColoursImportant = reader.ReadUInt32(),
					};

					uint fileSize = header.FileSize;
					uint dataSize = fileSize - header.DataOffset;

					byte[] data = new byte[dataSize];

					reader.BaseStream.Seek(header.DataOffset, SeekOrigin.Begin);

					uint dataRead = 0;
					uint rowSize = infoHeader.Width * (infoHeader.BitCount / 8);
					uint rowCount = dataSize / rowSize;
					while(dataRead < dataSize)
					{
						uint writePosition = dataSize - dataRead - rowSize;

						for (int i = 0; i < rowSize; ++i)
							data[writePosition + i] = reader.ReadByte();

						dataRead += rowSize;
					}

					return new Icon()
					{
						Filepath = filepath,
						Width = (ushort)infoHeader.Width,
						Height = (ushort)infoHeader.Height,
						Data = data,
					};
				}
			}
		}

		private static async Task SendChunks(Icon icon, Layout.MappedComponent mappedComponent, ActiveConnection connection)
        {
            int chunk = 0;
            int chunksize = 1024;

            while(chunk < icon.Data.Length)
            {
				try
				{
					chunk += await SendChunk(icon, mappedComponent, chunk, chunksize, connection);
				}
				catch(ConnectionClosedException)
				{
					// The device has been unplugged, we should stop sending this icon
					Debug.Print($"Device unplugged - cancelling send of '{icon.Filepath}' to device formally connected on '{connection.Name}'");
					break;
				}

                await Task.Delay(10);
            }
        }

        private static async Task<int> SendChunk(Icon icon, Layout.MappedComponent mappedComponent, int chunk, int chunksize, ActiveConnection connection)
		{
			GuaranteedPacket packet = new GuaranteedPacket();
			packet.Connection = connection;

			int segmentSize = Math.Min(chunksize, icon.Data.Length - chunk);
            ArraySegment<byte> segment = new ArraySegment<byte>(icon.Data, chunk, segmentSize);

            await packet.SendAsync(HardwareMonitor.Protocol.PacketType.ModuleUpdate, mappedComponent.ModuleIndex, (byte)1, mappedComponent.ComponentIndex, icon.Width, icon.Height, chunk, segment);

            return segmentSize;
		}

		#endregion Private Methods
	}
}
