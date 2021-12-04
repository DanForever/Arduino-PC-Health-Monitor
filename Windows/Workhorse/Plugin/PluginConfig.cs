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
using System.Xml.Serialization;

namespace HardwareMonitor.Plugin.Config
{
	[XmlType("Plugin")]
	public class Entry
	{
		#region Public Properties

		[XmlAttribute()]
		public string Name { get; set; }

		#endregion Public Properties
	}

	[XmlRoot("Plugins")]
	public class Config : ConfigBase<Config>
	{
		#region Public Properties
		
		[XmlElement("Plugin")]
		public List<Entry> Plugins { get; set; }

		#endregion Public Properties

		#region Public Methods

		public static void SaveDummy()
		{
			Config config = new Config()
			{
				Plugins = new List<Entry>()
				{
					new Entry() { Name = "AdditionalMemoryMetrics" },
				},
			};

			config.Save("example.plugins.xml");
		}

		#endregion Public Methods
	}
}
