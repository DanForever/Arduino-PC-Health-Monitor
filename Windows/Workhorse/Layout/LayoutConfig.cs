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

namespace HardwareMonitor.Layout
{
	public enum Unit
	{
		Celcius,
		Mhz,
		GB,
		Percent,
		FPS,
		MS
	}

	public enum Colour
	{
		Red		= 0xF800      /* 255,   0,   0 */,
		Blue	= 0x001F      /*   0,   0, 255 */,
		Green	= 0x07E0      /*   0, 255,   0 */,
		Yellow	= 0xFFE0      /* 255, 255,   0 */,
	}

	[XmlInclude(typeof(Border))]
	[XmlInclude(typeof(Text))]
	[XmlInclude(typeof(Metric))]
	[XmlInclude(typeof(Icon))]
	public partial class Component
	{
		#region Public Properties

		[XmlAttribute]
		public ushort X { get; set; }

		[XmlAttribute]
		public ushort Y { get; set; }

		#endregion Public Properties
	}

	public partial class Border : Component
	{
		#region Public Properties

		[XmlAttribute]
		public ushort Width { get; set; }
		
		[XmlAttribute]
		public ushort Height { get; set; }

		[XmlAttribute]
		public Colour Colour { get; set; }
		
		#endregion Public Properties
	}

	public partial class Icon : Component
	{
	}

	public partial class Text : Component
	{
		#region Public Properties

		[XmlAttribute]
		public byte TextSize { get; set; }

		#endregion Public Properties
	}

	public partial class Metric : Component
	{
		#region Public Properties

		[XmlAttribute]
		public byte TextSize { get; set; }

		[XmlAttribute]
		public byte UnitTextSize { get; set; }

		[XmlAttribute]
		public Unit Unit { get; set; }

		[XmlAttribute]
		public byte Precision { get; set; }

		#endregion Public Properties
	}

	public partial class Module
	{
		#region Public Properties

		[XmlAttribute]
		public ushort X { get; set; }

		[XmlAttribute]
		public ushort Y { get; set; }

		public List<Component> Components { get; set; }

		#endregion Public Properties
	}

	[XmlRoot("Layout")]
	public partial class Config : ConfigBase<Config>
	{
		#region Public Properties

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public Orientation Orientation { get; set; }

		[XmlAttribute]
		public eResolution Resolution { get; set; }

		public List<Module> Modules { get; set; }

		#endregion Public Properties

		#region Public Methods

		public static void SaveDummy()
		{
			Config config = new Config()
			{
				Modules = new List<Module>()
				{
					new Module()
					{
						X = 0,
						Y = 0,
						Components = new List<Component>()
						{
							new Border()
							{
								X = 0,
								Y = 0,
								Width = 240,
								Height = 120,
								Colour = Colour.Blue,
							},
							new Text()
							{
								X = 16,
								Y = 8,
								TextSize = 1,
							},
							new Metric
							{
								X = 105,
								Y = 25,
								TextSize = 3,
								UnitTextSize = 1,
								Unit = Unit.Celcius,
								Precision = 0,
							},
							new Metric
							{
								X = 105,
								Y = 55,
								TextSize = 3,
								UnitTextSize = 1,
								Unit = Unit.Mhz,
								Precision = 2,
							},
							new Metric
							{
								X = 105,
								Y = 85,
								TextSize = 3,
								UnitTextSize = 3,
								Unit = Unit.Percent,
								Precision = 0,
							}
						}
					}
				},
			};

			config.Save("example.layout.xml");
		}

		#endregion Public Methods
	}
}
