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
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace HardwareMonitor.Monitor.Config
{
	public abstract class CaptureDescriptor
	{
		#region Private fields

		private Regex _nameRegex = null;

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// The name of the hardware or sensor to look for. Regex (where appropriate)
		/// </summary>
		[XmlAttribute()]
		public string Name { get; set; }

		/// <summary>
		/// A unique Id used to identify this captured value from the layouts xml. Regex
		/// </summary>
		[XmlAttribute("CaptureAs")]
		public string CaptureName { get; set; }

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

		#endregion Public Properties
	}

	public class Component : CaptureDescriptor
	{
		#region Public Properties

		[XmlAttribute()]
		public HardwareType Type { get; set; }

		#endregion Public Properties
	}

	/// <summary>
	/// When polling the hardware, all the properties of a Sensor will be checked against, and if there's a match, it will be added to the snapshot
	/// </summary>
	public class Sensor : CaptureDescriptor
	{
		#region Public Properties

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

		#endregion Public Properties
	}

	public class CompoundSensor : CaptureDescriptor
	{
		#region Private fields

		private List<Sensor> _sensors = new List<Sensor>();

		#endregion Private fields

		#region Public Properties

		/// <summary>
		/// How the values of all the sensors will be aggregated
		/// </summary>
		[XmlAttribute()]
		public string Algorithm { get; set; }

		public List<Sensor> Sensors => _sensors;

		#endregion Public Properties
	}

	[XmlRoot()]
	public class Computer : ConfigBase<Computer>
	{
		#region Private fields

		private List<Component> _hardware = new List<Component>();
		private List<Sensor> _sensors = new List<Sensor>();
		private List<CompoundSensor> _compoundSensors = new List<CompoundSensor>();

		#endregion Private fields

		#region Public Properties

		public List<Component> Hardware => _hardware;
		public List<Sensor> Sensors => _sensors;
		public List<CompoundSensor> CompoundSensors => _compoundSensors;

		#endregion Public Properties

		#region Private Methods

		private static bool IsSensorSpecified(List<Sensor> sensors, string name, HardwareType component, SensorType sensorType)
		{
			return sensors.Exists((Sensor sensor) => sensor.Name == name && sensor.Component == component && sensor.Type == sensorType);
		}
		
		#endregion Private Methods

		#region Public Methods

		public virtual bool IsHardwareSpecified(HardwareType type)
		{
			return Hardware.Exists((Component component) => component.Type == type);
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

		#endregion Public Methods
	}

	/// <summary>
	/// Pretends to request all data types so that we can dump a complete list of statistics to a file or string
	/// </summary>
	internal class DummyComputer : Computer
	{
		#region Computer

		public override bool IsHardwareSpecified(HardwareType type)
		{
			return true;
		}

		#endregion Computer
	}
}
