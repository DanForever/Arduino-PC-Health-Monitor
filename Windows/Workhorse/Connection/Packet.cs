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
using System.Text;

namespace HardwareMonitor.Connection
{
	internal abstract class Packet
	{
		#region Public consts

		// @todo: Consolidate
		public static byte[] MessageHeader => Encoding.ASCII.GetBytes("dan<`");
		public static byte[] MessageFooter => Encoding.ASCII.GetBytes("`>dan");


		#endregion Public consts

		#region Public Properties

		public IList<ActiveConnection> Connections { get; set; }

		#endregion Public Properties

		#region Protected Methods

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

		#endregion Protected Methods

		#region Abstract Methods

		public abstract void Send(params dynamic[] args);

		#endregion Abstract Methods
	}
}
