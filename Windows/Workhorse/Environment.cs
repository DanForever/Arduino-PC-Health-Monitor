/*
 *   Arduino PC Health Monitor (PC Companion app)
 *   Polls the hardware sensors for data and forwards them on to the arduino device
 *   Copyright (C) 2022 Daniel Neve
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

namespace HardwareMonitor
{
	public static class Environment
	{
		public static string UserfolderName => "DansHardwareMonitor";

		public static string RoamingAppdata => System.IO.Path.Join(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), UserfolderName);
		public static string LocalAppdata => System.IO.Path.Join(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), UserfolderName);

		public static string ExecutablePath => System.Reflection.Assembly.GetEntryAssembly().Location;
		public static string ExecutableFolder => System.IO.Path.GetDirectoryName(ExecutablePath);
	}
}
