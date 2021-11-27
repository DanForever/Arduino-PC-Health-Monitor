using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Libre = LibreHardwareMonitor.Hardware;

namespace HardwareMonitor.Monitor.Asynchronous
{
	public static class Watcher
	{
		public static async Task<Snapshot> Poll(Config.Computer config, IEnumerable<Plugin.ISource> sources)
		{
			return await Task.Run(() => Synchronous.Watcher.Poll(config, sources));
		}
	}
}

namespace HardwareMonitor.Monitor.Synchronous
{
	internal static class LibreExtensions
	{
		private static Dictionary<Libre.HardwareType, HardwareType> _hardwareTypeMap = new Dictionary<Libre.HardwareType, HardwareType>()
		{
			{Libre.HardwareType.Motherboard         ,HardwareType.Motherboard},
			//{Libre.HardwareType.SuperIO				,HardwareType.},
			{Libre.HardwareType.Cpu                 ,HardwareType.Cpu},
			{Libre.HardwareType.Memory              ,HardwareType.Memory},
			{Libre.HardwareType.GpuNvidia           ,HardwareType.Gpu},
			{Libre.HardwareType.GpuAmd              ,HardwareType.Gpu},
			{Libre.HardwareType.Storage             ,HardwareType.Storage},
			{Libre.HardwareType.Network             ,HardwareType.Network},
			//{Libre.HardwareType.Cooler				,HardwareType.c},
			//{Libre.HardwareType.EmbeddedController	,HardwareType.},
			//{Libre.HardwareType.Psu					,HardwareType.},
		};

		private static Dictionary<Libre.SensorType, SensorType> _sensorTypeMap = new Dictionary<Libre.SensorType, SensorType>()
		{
			//{Libre.SensorType.Voltage,			SensorType.},
			//{Libre.SensorType.Current,			SensorType.},
			//{Libre.SensorType.Power,			 SensorType.},
			{Libre.SensorType.Clock,             SensorType.Clock},
			{Libre.SensorType.Temperature,       SensorType.Temperature},
			{Libre.SensorType.Load,              SensorType.Load},
			//{Libre.SensorType.Frequency,		 SensorType.},
			//{Libre.SensorType.Fan,				SensorType.},
			//{Libre.SensorType.Flow,				SensorType.},
			//{Libre.SensorType.Control,			SensorType.},
			//{Libre.SensorType.Level,			 SensorType.},
			//{Libre.SensorType.Factor,			 SensorType.},
			{Libre.SensorType.Data,             SensorType.Data},
			//{Libre.SensorType.SmallData,		 SensorType.},
			//{Libre.SensorType.Throughput,		 SensorType.},
			//{Libre.SensorType.TimeSpan,			SensorType.},
		};

		public static HardwareType Convert(this Libre.HardwareType hardwareType)
		{
			return _hardwareTypeMap.GetValueOrDefault(hardwareType, HardwareType.Ignored);
		}

		public static SensorType Convert(this Libre.SensorType sensorType)
		{
			return _sensorTypeMap.GetValueOrDefault(sensorType, SensorType.Ignored);
		}
	}

	public static class Watcher
	{
		private class CompoundSensorData
		{
			private List<SensorSample> _samples = new List<SensorSample>();

			public List<SensorSample> Samples => _samples;

			public Config.CompoundSensor Sensor { get; set; }
		}

		private class PollData
		{
			public Config.Computer Config { get; set; }
			public IEnumerable<Plugin.ISource> Sources { get; set; }
			public Dictionary<string, CompoundSensorData> CompoundSensors { get; set; }
			public Snapshot Snapshot { get; set; }
		}

		private static PollData _data = new PollData();

		public static Snapshot Poll(Config.Computer config, IEnumerable<Plugin.ISource> sources)
		{
			_data.Config = config;
			_data.Sources = sources;
			_data.CompoundSensors = new Dictionary<string, CompoundSensorData>();
			_data.Snapshot = new Snapshot();

			Libre.Computer computer = new Libre.Computer
			{
				IsCpuEnabled = config.IsHardwareSpecified(HardwareType.Cpu),
				IsGpuEnabled = config.IsHardwareSpecified(HardwareType.Gpu),
				IsMemoryEnabled = config.IsHardwareSpecified(HardwareType.Memory),
				IsMotherboardEnabled = config.IsHardwareSpecified(HardwareType.Motherboard),
				IsNetworkEnabled = config.IsHardwareSpecified(HardwareType.Network),
				IsStorageEnabled = config.IsHardwareSpecified(HardwareType.Storage),
				IsControllerEnabled = false,
				IsPsuEnabled = false,
			};

			computer.Open();
			computer.Accept(new UpdateVisitor());

			PollHardware(computer.Hardware, _data);
			computer.Close();

			PollPlugins(_data);
			ProcessCompoundSensors(_data);

			return _data.Snapshot;
		}

		#region Libre

		private static void PollHardware(IEnumerable<Libre.IHardware> hardware, PollData data)
		{
			foreach (Libre.IHardware hardwareItem in hardware)
			{
				Config.Component watchedHardware = data.Config.FindComponent(hardwareItem.HardwareType.Convert());
				if (watchedHardware == null)
					continue;

				data.Snapshot.HardwareSamples.Add(new HardwareSample() { Component = watchedHardware, Name = hardwareItem.Name });

				if (!string.IsNullOrWhiteSpace(watchedHardware.CaptureName))
					data.Snapshot.Captures.Add(watchedHardware.CaptureName, new Capture() { Name = watchedHardware.CaptureName, Value = hardwareItem.Name });

				PollSensors(hardwareItem, data);
			}
		}

		private static void PollSensors(Libre.IHardware hardware, PollData data)
		{
			PollHardware(hardware.SubHardware, data);

			foreach (Libre.ISensor sensor in hardware.Sensors)
			{
				CheckSensor(sensor, data);
				CheckCompoundSensor(sensor, data);
			}
		}

		private static void CheckSensor(Libre.ISensor sensor, PollData data)
		{
			Config.Sensor watchedSensor = data.Config.FindSensor(sensor.Name, sensor.Hardware.HardwareType.Convert(), sensor.SensorType.Convert());
			if (watchedSensor == null)
				return;

			data.Snapshot.SensorSamples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value ?? 0.0f });

			if (!string.IsNullOrWhiteSpace(watchedSensor.CaptureName))
				data.Snapshot.Captures.Add(watchedSensor.CaptureName, new Capture() { Name = watchedSensor.CaptureName, Value = sensor.Value ?? 0.0f });
		}

		private static void CheckCompoundSensor(Libre.ISensor sensor, PollData data)
		{
			Config.CompoundSensor watchedCompoundSensor = data.Config.FindCompoundSensor(sensor.Name, sensor.Hardware.HardwareType.Convert(), sensor.SensorType.Convert());
			if (watchedCompoundSensor == null)
				return;

			CompoundSensorData compoundSensorData;
			if(!data.CompoundSensors.TryGetValue(watchedCompoundSensor.Name, out compoundSensorData))
			{
				compoundSensorData = new CompoundSensorData() { Sensor = watchedCompoundSensor };
				data.CompoundSensors.Add(watchedCompoundSensor.Name, compoundSensorData);
			}

			Config.Sensor watchedSensor = data.Config.FindSensor(watchedCompoundSensor.Sensors, sensor.Name, sensor.Hardware.HardwareType.Convert(), sensor.SensorType.Convert());
			compoundSensorData.Samples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value ?? 0.0f });
		}

		#endregion Libre

		#region Plugins

		private static void PollPlugins(PollData data)
		{
			foreach (Plugin.ISource source in data.Sources)
			{
				source.Update();

				PollHardware(source.Hardware, data);
			}
		}

		private static void PollHardware(IEnumerable<Plugin.IHardware> hardware, PollData data)
		{
			foreach (Plugin.IHardware hardwareItem in hardware)
			{
				Config.Component watchedHardware = data.Config.FindComponent(hardwareItem.Type);
				if (watchedHardware == null)
					continue;

				data.Snapshot.HardwareSamples.Add(new HardwareSample() { Component = watchedHardware, Name = hardwareItem.Name });

				if (!string.IsNullOrWhiteSpace(watchedHardware.CaptureName))
					data.Snapshot.Captures[watchedHardware.CaptureName] = new Capture() { Name = watchedHardware.CaptureName, Value = hardwareItem.Name };

				PollSensors(hardwareItem, data);
			}
		}

		private static void PollSensors(Plugin.IHardware hardware, PollData data)
		{
			foreach (Plugin.ISensor sensor in hardware.Sensors)
			{
				/*
				Config.Sensor watchedSensor = Config.FindSensor(sensor.Name, sensor.Hardware.Type, sensor.Type);
				if (watchedSensor == null)
					continue;

				snapshot.SensorSamples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value ?? 0.0f });

				if (!string.IsNullOrWhiteSpace(watchedSensor.CaptureName))
					snapshot.Captures.Add(watchedSensor.CaptureName, new Capture() { Name = watchedSensor.CaptureName, Value = sensor.Value ?? 0.0f });
				//*/
			}
		}


		#endregion Plugins

		private static void ProcessCompoundSensors(PollData data)
		{
			foreach(var compoundSensorData in data.CompoundSensors.Values)
			{
				string algorithmName = compoundSensorData.Sensor.Algorithm;
				string fullTypeName = $"{nameof(HardwareMonitor)}.{nameof(Algorithms)}.{nameof(Algorithms.Compound)}.{algorithmName}";

				Type algorithmType = Type.GetType(fullTypeName);

				// @todo: proper error messaging and handling
				if (algorithmType == null)
					continue;

				if (string.IsNullOrWhiteSpace(compoundSensorData.Sensor.CaptureName))
					continue;

				if (!typeof(Algorithms.Compound.IAlgorithm).IsAssignableFrom(algorithmType))
					continue;

				// @todo: cache the instances in a dictionary so we're not creating them all the time
				Algorithms.Compound.IAlgorithm instance = (Algorithms.Compound.IAlgorithm)Activator.CreateInstance(algorithmType);

				float result = instance.Calculate(from sample in compoundSensorData.Samples select sample.Value);

				data.Snapshot.Captures.Add(compoundSensorData.Sensor.CaptureName, new Capture() { Name = compoundSensorData.Sensor.CaptureName, Value = result });
			}
		}
	}
}
