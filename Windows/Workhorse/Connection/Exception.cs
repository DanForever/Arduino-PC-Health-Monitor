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

namespace HardwareMonitor.Connection
{
	[Serializable]
	public class ConnectionFailedException : Exception
	{
		#region Public Properties

		public AvailableConnection AvailableConnection { get; set; }

		#endregion Public Properties

		#region C-Tor

		public ConnectionFailedException(string reason) : base(reason)
		{
		}

		#endregion C-Tor
	}

	[Serializable]
	public class ConnectionClosedException : Exception
	{
		#region private Fields

		private ActiveConnection _connection;

		#endregion private Fields

		#region Public Properties

		public ActiveConnection Connection => _connection;

		#endregion Public Properties

		#region C-Tor

		public ConnectionClosedException(ActiveConnection connection) : base($"Connection with name '{connection.Name}' is closed")
		{
			_connection = connection;
		}

		#endregion C-Tor
	}
}
