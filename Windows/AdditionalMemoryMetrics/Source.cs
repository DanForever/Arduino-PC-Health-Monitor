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
using System.Management;

using AdditionalMemoryMetrics.Hardware;

using HardwareMonitor.Plugin;

namespace AdditionalMemoryMetrics
{
	public class Source : ISource
	{
		private Memory[] _hardware = new Memory[1];

		IEnumerable<IHardware> ISource.Hardware => _hardware;

		void ISource.Update()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
			foreach (ManagementObject obj in searcher.Get())
			{
				string manufacturer = (string)obj.Properties["Manufacturer"].Value;
				string productCode = (string)obj.Properties["PartNumber"].Value;

				_hardware[0] = new Memory(productCode);

				// We only care about the first result we find
				// If there are non-matched ram sticks in this machine ¯\_(ツ)_/¯
				break;
			}
		}
	}
}
