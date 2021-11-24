using System.Xml;
using System.Xml.Serialization;

namespace HardwareMonitor.Protocol
{
	public class Capture
	{
		[XmlAttribute()]
		public string Name { get; set; }
	}
}
