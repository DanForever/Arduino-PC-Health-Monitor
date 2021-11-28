using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HardwareMonitor.Icon
{
	[XmlType("Icon")]
	public class Mapping
	{
		private Regex _hardwareRegex = null;

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
	}

	[XmlRoot()]
	public class Config : ConfigBase<Config>
	{
		public List<Mapping> IconMappings { get; set; }

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
	}
}
