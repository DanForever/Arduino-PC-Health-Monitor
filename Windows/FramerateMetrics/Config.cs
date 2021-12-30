using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FramerateMetrics.Config
{
	public class Program
	{
		#region Private fields

		private Regex _processNameRegex = null;

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// This is what will be used to detect if this program is running
		/// </summary>
		[XmlAttribute]
		public string ProcessName { get; set; }

		[XmlIgnore]
		public Regex ProcessNameRegex
		{
			get
			{
				if (_processNameRegex == null)
					_processNameRegex = new Regex(ProcessName);

				return _processNameRegex;
			}
		}

		#endregion Public Properties
	}

	[XmlRoot("FramerateMetrics")]
	public class Config : HardwareMonitor.ConfigBase<Config>
	{
		#region Public Properties

		public List<Program> Blacklist { get; set; }

		#endregion Public Properties

		#region Static Methods

		public static void SaveDummy()
		{
			Config config = new();

			config.Blacklist = new List<Program>
			{
				new Program { ProcessName = "iCUE" },
				new Program { ProcessName = "chrome" },
				new Program { ProcessName = "devenv" }
			};

			config.Save("example.frameratemetrics.xml");
		}
		
		#endregion Static Methods
	}
}
