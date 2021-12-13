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
	}

	public enum Colour
	{
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
		[XmlAttribute]
		public ushort X { get; set; }

		[XmlAttribute]
		public ushort Y { get; set; }
	}

	public partial class Border : Component
	{
		[XmlAttribute]
		public ushort Width { get; set; }
		
		[XmlAttribute]
		public ushort Height { get; set; }

		[XmlAttribute]
		public Colour Colour { get; set; }
	}

	public partial class Icon : Component
	{
	}

	public partial class Text : Component
	{
		[XmlAttribute]
		public byte TextSize { get; set; }
	}

	public partial class Metric : Component
	{
		[XmlAttribute]
		public byte TextSize { get; set; }

		[XmlAttribute]
		public byte UnitTextSize { get; set; }

		[XmlAttribute]
		public Unit Unit { get; set; }

		[XmlAttribute]
		public byte Precision { get; set; }
	}

	public partial class Module
	{
		[XmlAttribute]
		public ushort X { get; set; }

		[XmlAttribute]
		public ushort Y { get; set; }

		public List<Component> Components { get; set; }
	}

	[XmlRoot("Layout")]
	public partial class Config : ConfigBase<Config>
	{
		// @todo: Orientation, resolution

		public List<Module> Modules { get; set; }

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
	}
}
