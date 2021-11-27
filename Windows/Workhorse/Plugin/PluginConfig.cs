using System.Collections.Generic;
using System.Xml.Serialization;

namespace HardwareMonitor.Plugin.Config
{
	[XmlType("Plugin")]
	public class Entry
	{
		[XmlAttribute()]
		public string Name { get; set; }
	}

	[XmlRoot("Plugins")]
	public class Config : ConfigBase<Config>
	{
		[XmlElement("Plugin")]
		public List<Entry> Plugins { get; set; }

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
	}
}
