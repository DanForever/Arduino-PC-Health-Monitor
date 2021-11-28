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
using System.Xml.Serialization;

namespace HardwareMonitor.Protocol
{
	public class Metric
	{
		[XmlAttribute()]
		public Metrics Type { get; set; }

		[XmlAttribute()]
		public string Capture { get; set; }

		[XmlAttribute]
		public bool NoUpdate { get; set; } = false;
	}

	public class Module
	{
		[XmlArray()]
		public List<Metric> Metrics { get; set; }
	}

	[XmlRoot("Protocol")]
	public class Config
	{
		[XmlArray()]
		public List<Module> Modules { get; set; }

		public static Config Load(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Config));

			using (TextReader reader = new StreamReader(filename))
			{
				return (Config)serializer.Deserialize(reader);
			}
		}

		public void Save(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Config));

			using (TextWriter writer = new StreamWriter(filename))
			{
				serializer.Serialize(writer, this);
			}
		}

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
	}
}
