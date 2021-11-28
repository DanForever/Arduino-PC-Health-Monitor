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
using System.IO;
using System.Threading.Tasks;

namespace HardwareMonitor.Connection
{
	namespace Bitmap
	{
		class Header
		{
			public ushort Signature { get; set; }
			public uint FileSize { get; set; }
			public uint Reserved { get; set; }
			public uint DataOffset { get; set; }
		}

		class InfoHeader
		{
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
		}
	}

    public class Icon
    {
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public byte[] Data { get; set; }
    }

    public static class IconSender
    {
        public static async void Send(Protocol.Metrics metric, string filepath, Device device)
        {
            Icon icon = await Load(filepath);

            if (icon == null)
                return;

            await SendChunks(icon, metric, device);
        }

		public static async void DebugSend(int width, int height, byte[] data, Protocol.Metrics metric, Device device)
		{
			await SendChunks(new Icon() { Width = (ushort)width, Height = (ushort)height, Data = data }, metric, device);
		}

		private static async Task<Icon> Load(string filepath)
		{
			return await Task.Run(() => LoadSync(filepath));
		}

        private static Icon LoadSync(string filepath)
        {
			//Stream imageStreamSource = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

			//BmpBitmapDecoder decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			//BitmapSource bitmapSource = decoder.Frames[0];

			//if (bitmapSource.Format != System.Windows.Media.PixelFormats.Bgr565)
			//{
			//    Debug.Print($"Image at path `{filepath}` is not RGB565!");
			//    return null;
			//}

			//byte[] icon = new byte[bitmapSource.PixelHeight * bitmapSource.PixelWidth * sizeof(short)];

			//bitmapSource.CopyPixels(icon, bitmapSource.PixelWidth * sizeof(short), 0);

			//return new Icon() { Width = (ushort)bitmapSource.PixelWidth, Height = (ushort)bitmapSource.PixelHeight, Data = icon };

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
						Width = (ushort)infoHeader.Width,
						Height = (ushort)infoHeader.Height,
						Data = data,
					};
				}
			}
		}

		private static async Task SendChunks(Icon icon, Protocol.Metrics metric, Device device)
        {
            int chunk = 0;
            int chunksize = 1024;

            while(chunk < icon.Data.Length)
            {
                chunk += await SendChunk(icon, metric, chunk, chunksize, device);

                await Task.Delay(10);
            }
        }

        private static async Task<int> SendChunk(Icon icon, Protocol.Metrics metric, int chunk, int chunksize, Device device)
		{
			GuaranteedPacket packet = new GuaranteedPacket();
			packet.Connections = new List<ActiveConnection>() { device.Connection };

			int segmentSize = Math.Min(chunksize, icon.Data.Length - chunk);
            ArraySegment<byte> segment = new ArraySegment<byte>(icon.Data, chunk, segmentSize);

            await packet.SendAsync(Protocol.PacketType.SensorUpdate, metric, icon.Width, icon.Height, chunk, segment);

            return segmentSize;
        }
    }
}
