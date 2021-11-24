using System.Collections.Generic;

namespace HardwareMonitor.Monitor
{
	/// <summary>
	/// Contains the values obtained from a single poll of the hardware. Immutable
	/// </summary>
	public class SensorSample
	{
		/// <summary>
		/// The configuration that triggered this particular value to be captured
		/// </summary>
		public Config.Sensor Sensor { get; set; }

		/// <summary>
		/// The name of the sensor as reported by the hardware
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The value as reported by the sensor.
		/// </summary>
		/// <remarks>
		/// Depending on the sensor type it could be in Celcius, Gigabytes, Mhz, etc
		/// </remarks>
		public float Value { get; set; }
	}

	public class HardwareSample
	{
		public Config.Component Component { get; set; }

		public string Name { get; set; }
	}


	public class Capture
	{
		/// <summary>
		/// The unique name of this capture
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The value of this particular capture
		/// </summary>
		/// <example>
		/// For a Sensor, this will be the value of that particular sensor
		/// For Hardware, this will be the name given by that component
		/// </example>
		public dynamic Value { get; set; }
	}

	public class Snapshot
	{
		private List<HardwareSample> _hardwareSamples = new List<HardwareSample>();
		private List<SensorSample> _sensorSamples = new List<SensorSample>();
		private Dictionary<string, Capture> _captures = new Dictionary<string, Capture>();

		public List<HardwareSample> HardwareSamples => _hardwareSamples;
		public List<SensorSample> SensorSamples => _sensorSamples;
		public Dictionary<string, Capture> Captures => _captures;
	}
}
