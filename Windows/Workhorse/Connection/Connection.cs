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

namespace HardwareMonitor.Connection
{
	public abstract class AvailableConnection
	{
		#region Abstract Properties

		public abstract string Name { get; }

		#endregion Abstract Properties

		#region Abstract Methods

		public abstract ActiveConnection Connect();

		#endregion Abstract Methods
	}

	public interface ActiveConnection
	{
		#region Interface Events

		public delegate void DataRecievedHandler(ActiveConnection connection, byte[] data, int dataLength);
		public event DataRecievedHandler DataRecieved;

		#endregion Interface Events
		
		#region Interface Properties

		public string Name { get; }

		public bool IsOpen { get; }

		#endregion Interface Properties

		#region Interface Methods

		public void Send(byte[] data);

		#endregion Interface Methods
	}
}
