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

namespace HardwareMonitor.Connection
{
	static class Connections
	{
		#region Public Methods

		public static IEnumerable<AvailableConnection> Enumerate()
		{
			// Iterate over all serial ports
			foreach (AvailableConnection availableConnection in Serial.Connections.EnumeratePorts())
			{
				yield return availableConnection;
			}

			// Iterate over every other type of connection
		}

		#endregion Public Methods
	}
}
