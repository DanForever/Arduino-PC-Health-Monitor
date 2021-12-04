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

namespace HardwareMonitor.Protocol
{
	[XmlInclude(typeof(Icon))]
	public class Metric
	{
		#region Public Properties

		[XmlAttribute()]
		public Metrics Type { get; set; }

		[XmlAttribute()]
		public string Capture { get; set; }

		[XmlAttribute]
		public bool NoUpdate { get; set; } = false;


		#endregion Public Properties
	}

	public class Icon : Metric
	{
		#region C-Tor

		public Icon() { NoUpdate = true; }
		
		#endregion C-Tor
	}

	public class Module
	{
		#region Public Properties

		[XmlArray()]
		public List<Metric> Metrics { get; set; }

		#endregion Public Properties
	}

	[XmlRoot("Protocol")]
	public class Config : ConfigBase<Config>
	{
		#region Public Properties

		[XmlArray()]
		public List<Module> Modules { get; set; }

		#endregion Public Properties

		#region Public Methods

		public static void SaveDummy()
		{
			var config = new Protocol.Config()
			{
				Modules = new List<Module>
				{
					new Protocol.Module()
					{
						Metrics = new List<Metric>
						{
							new Protocol.Metric() { Type = Protocol.Metrics.CpuName, Capture="CpuName", NoUpdate = true },
							new Protocol.Metric() { Type = Protocol.Metrics.CpuTotalLoad, Capture="CpuLoad" },
						},
					},
					new Protocol.Module()
					{
						Metrics = new List<Metric>
						{
							new Protocol.Metric() { Type = Protocol.Metrics.GpuName, Capture="GpuName", NoUpdate = true },
						},
					}
				}
			};

			config.Save("example.protocol.xml");
		}

		#endregion Public Methods
	}
}
