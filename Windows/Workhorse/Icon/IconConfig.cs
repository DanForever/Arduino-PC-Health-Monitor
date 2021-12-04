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

namespace HardwareMonitor.Icon
{
	[XmlType("Icon")]
	public class Mapping
	{
		#region Private fields

		private Regex _hardwareRegex = null;

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// The name of the hardware to match (Regex)
		/// </summary>
		[XmlAttribute()]
		public string Hardware { get; set; }

		[XmlAttribute()]
		public string Icon { get; set; }

		[XmlIgnore()]
		public Regex HardwareRegex
		{
			get
			{
				if (_hardwareRegex == null)
					_hardwareRegex = new Regex(Hardware);

				return _hardwareRegex;
			}
		}

		#endregion Public Properties
	}

	[XmlRoot()]
	public class Config : ConfigBase<Config>
	{
		#region Public Properties

		public List<Mapping> IconMappings { get; set; }

		#endregion Public Properties

		#region Public Methods

		public string GetIcon(string name)
		{
			foreach(Mapping mapping in IconMappings)
			{
				if (mapping.HardwareRegex.IsMatch(name))
					return mapping.Icon;
			}

			return null;
		}

		public static void SaveDummy()
		{
			Config config = new Config()
			{
				IconMappings = new List<Mapping>()
				{
					new Mapping() { Hardware = "Ryzen", Icon = "ryzen_black" },
				},
			};

			config.Save("example.icons.xml");
		}

		#endregion Public Methods
	}
}
