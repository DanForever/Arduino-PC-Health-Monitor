using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace HardwareMonitor.Monitor.Config
{
	public abstract class CaptureDescriptor
	{
		[XmlAttribute("CaptureAs")]
		public string CaptureName { get; set; }
	}

	public class Component : CaptureDescriptor
	{
		[XmlAttribute()]
		public HardwareType Type { get; set; }
	}

	/// <summary>
	/// When polling the hardware, all the properties of a Sensor will be checked against, and if there's a match, it will be added to the snapshot
	/// </summary>
	public class Sensor : CaptureDescriptor
	{
		/// <summary>
		/// The name of the sensor to look for. Regex
		/// </summary>
		[XmlAttribute()]
		public string Name { get; set; }

		/// <summary>
		/// The type of hardware the sensor is tied to
		/// </summary>
		[XmlAttribute()]
		public HardwareType Component { get; set; }

		/// <summary>
		/// The type of sensor to look for
		/// </summary>
		[XmlAttribute()]
		public SensorType Type { get; set; }

		private Regex _nameRegex = null;

		[XmlIgnore()]
		public Regex NameRegex
		{
			get
			{
				if (_nameRegex == null)
					_nameRegex = new Regex(Name);

				return _nameRegex;
			}
		}
	}

	public class CompoundSensor : CaptureDescriptor
	{
		#region Private fields

		private List<Sensor> _sensors = new List<Sensor>();

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// User defined name
		/// </summary>
		/// <remarks>
		/// The compount sensor will appear as a regular sensor in the snapshot, so this is used as the display name
		/// </remarks>
		[XmlAttribute()]
		public string Name { get; set; }

		/// <summary>
		/// How the values of all the sensors will be aggregated
		/// </summary>
		[XmlAttribute()]
		public string Algorithm { get; set; }

		public List<Sensor> Sensors => _sensors;

		#endregion Public Properties
	}

	[XmlRoot()]
	public class Computer
	{
		private List<Component> _hardware = new List<Component>();
		private List<Sensor> _sensors = new List<Sensor>();
		private List<CompoundSensor> _compoundSensors = new List<CompoundSensor>();

		public List<Component> Hardware => _hardware;
		public List<Sensor> Sensors => _sensors;
		public List<CompoundSensor> CompoundSensors => _compoundSensors;

		public bool IsHardwareSpecified(HardwareType type)
		{
			return Hardware.Exists((Component component) => component.Type == type);
		}

		private static bool IsSensorSpecified(List<Sensor> sensors, string name, HardwareType component, SensorType sensorType)
		{
			return sensors.Exists((Sensor sensor) => sensor.Name == name && sensor.Component == component && sensor.Type == sensorType);
		}

		public bool IsSensorSpecified(string name, HardwareType component, SensorType sensorType)
		{
			return IsSensorSpecified(Sensors, name, component, sensorType) || CompoundSensors.Exists((CompoundSensor compoundSensor) => IsSensorSpecified(compoundSensor.Sensors, name, component, sensorType));
		}

		public Component FindComponent(HardwareType type)
        {
			return Hardware.Find((Component component) => component.Type == type);
		}

		public Sensor FindSensor(List<Sensor> sensors, string name, HardwareType component, SensorType sensorType)
		{
			return sensors.Find((Sensor sensor) => sensor.NameRegex.IsMatch(name) && sensor.Component == component && sensor.Type == sensorType);
		}

		public Sensor FindSensor(string name, HardwareType component, SensorType sensorType)
		{
			Sensor sensor = FindSensor(Sensors, name, component, sensorType);
			if (sensor != null)
				return sensor;

			return null;
		}

		public CompoundSensor FindCompoundSensor(string name, HardwareType component, SensorType sensorType)
		{
			foreach (CompoundSensor compoundSensor in CompoundSensors)
			{
				Sensor subSensor = FindSensor(compoundSensor.Sensors, name, component, sensorType);
				if (subSensor != null)
					return compoundSensor;
			}

			return null;
		}

		public static Computer Load(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Computer));

			using (TextReader reader = new StreamReader(filename))
			{
				return (Computer)serializer.Deserialize(reader);
			}
		}

		public void Save(string filename)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Computer));

			using(TextWriter writer = new StreamWriter(filename))
			{
				serializer.Serialize(writer, this);
			}
		}

		public static void SaveDummy()
		{
			Computer config = new Computer();

			config.Hardware.Add(new Component() { Type = HardwareType.Cpu });
			config.Hardware.Add(new Component() { Type = HardwareType.Gpu });
			config.Hardware.Add(new Component() { Type = HardwareType.Memory });

			config.Sensors.Add(new Sensor() { Name = "CPU Total", Component = HardwareType.Cpu, Type = SensorType.Load });
			config.Sensors.Add(new Sensor() { Name = "Core (Tctl/Tdie)", Component = HardwareType.Cpu, Type = SensorType.Temperature });

			CompoundSensor clockSensor = new CompoundSensor() { Algorithm = "Average" };
			clockSensor.Sensors.Add(new Sensor() { Name = @"Core #(\d+)", Component = HardwareType.Cpu, Type = SensorType.Clock });
			config.CompoundSensors.Add(clockSensor);

			config.Save("example.computer.xml");
		}
	}
}
