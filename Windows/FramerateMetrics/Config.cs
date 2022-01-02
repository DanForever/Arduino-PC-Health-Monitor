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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FramerateMetrics.Config
{
	public class Program
	{
		#region Private fields

		private Regex _processNameRegex = null;

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// This is what will be used to detect if this program is running
		/// </summary>
		[XmlAttribute]
		public string ProcessName { get; set; }

		[XmlIgnore]
		public Regex ProcessNameRegex
		{
			get
			{
				if (_processNameRegex == null)
					_processNameRegex = new Regex(ProcessName);

				return _processNameRegex;
			}
		}

		#endregion Public Properties
	}

	[XmlRoot("FramerateMetrics")]
	public class Config : HardwareMonitor.ConfigBase<Config>
	{
		#region Public Properties

		public List<Program> Blacklist { get; set; }

		#endregion Public Properties

		#region Static Methods

		public static void SaveDummy()
		{
			Config config = new();

			config.Blacklist = new List<Program>
			{
				new Program { ProcessName = "iCUE" },
				new Program { ProcessName = "chrome" },
				new Program { ProcessName = "devenv" }
			};

			config.Save("example.frameratemetrics.xml");
		}
		
		#endregion Static Methods
	}
}
