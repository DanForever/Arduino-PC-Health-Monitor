using System.Collections.Generic;
using System.Threading.Tasks;

using Libre = LibreHardwareMonitor.Hardware;

namespace HardwareMonitor.Monitor.Asynchronous
{
	public static class Watcher
	{
		public static async Task<Snapshot> Poll(Config.Computer config)
		{
			return await Task.Run(() => Synchronous.Watcher.Poll(config));
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
			{Libre.SensorType.Data,				SensorType.Data},
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
		public static Config.Computer Config { get; set; }

		public static Snapshot Poll(Config.Computer config)
		{
			Config = config;

			Libre.Computer computer = new Libre.Computer
			{
				IsCpuEnabled = Config.IsHardwareSpecified(HardwareType.Cpu),
				IsGpuEnabled = Config.IsHardwareSpecified(HardwareType.Gpu),
				IsMemoryEnabled = Config.IsHardwareSpecified(HardwareType.Memory),
				IsMotherboardEnabled = Config.IsHardwareSpecified(HardwareType.Motherboard),
				IsNetworkEnabled = Config.IsHardwareSpecified(HardwareType.Network),
				IsStorageEnabled = Config.IsHardwareSpecified(HardwareType.Storage),
				IsControllerEnabled = false,
				IsPsuEnabled = false,
			};

			computer.Open();
			computer.Accept(new UpdateVisitor());

			Snapshot snapshot = new Snapshot();
			PollHardware(computer.Hardware, snapshot);
			computer.Close();

			return snapshot;
		}

		private static void PollHardware(IEnumerable<Libre.IHardware> hardware, Snapshot snapshot)
		{
			foreach (Libre.IHardware hardwareItem in hardware)
			{
				Config.Component watchedHardware = Config.FindComponent(hardwareItem.HardwareType.Convert());
				if (watchedHardware == null)
					continue;

				snapshot.HardwareSamples.Add(new HardwareSample() { Component = watchedHardware, Name = hardwareItem.Name });

				if (!string.IsNullOrWhiteSpace(watchedHardware.CaptureName))
					snapshot.Captures.Add(watchedHardware.CaptureName, new Capture() { Name = watchedHardware.CaptureName, Value = hardwareItem.Name });

				PollSensors(hardwareItem, snapshot);
			}
		}

		private static void PollSensors(Libre.IHardware hardware, Snapshot snapshot)
		{
			PollHardware(hardware.SubHardware, snapshot);

			foreach (Libre.ISensor sensor in hardware.Sensors)
			{
				Config.Sensor watchedSensor = Config.FindSensor(sensor.Name, sensor.Hardware.HardwareType.Convert(), sensor.SensorType.Convert());
				if (watchedSensor == null)
					continue;

				snapshot.SensorSamples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value ?? 0.0f });

				if (!string.IsNullOrWhiteSpace(watchedSensor.CaptureName))
					snapshot.Captures.Add(watchedSensor.CaptureName, new Capture() { Name = watchedSensor.CaptureName, Value = sensor.Value ?? 0.0f });
			} 
		}
	}
}
