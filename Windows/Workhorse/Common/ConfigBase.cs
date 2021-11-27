using System.IO;
using System.Xml.Serialization;

namespace HardwareMonitor
{
	public abstract class ConfigBase<T> where T : ConfigBase<T>
	{
		public static T Load(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));

			using (TextReader reader = new StreamReader(filename))
			{
				return (T)serializer.Deserialize(reader);
			}
		}

		public void Save(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));

			using (TextWriter writer = new StreamWriter(filename))
			{
				serializer.Serialize(writer, this);
			}
		}
	}
}
