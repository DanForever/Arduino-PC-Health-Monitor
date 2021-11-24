using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

/*
using LibreHardwareMonitor.Hardware;

namespace HardwareMonitor.Monitor
{
	class RegisteredSensorTypeOfInterest
	{
		private List<WatchedSensor> _sensors;

		public WatchedSensor this[int i] => _sensors[i];
		public WatchedSensor this[string name] => SensorOfInterest(name);

		public bool IsEmpty => _sensors.Count == 0;

		public void Add(WatchedSensor sensor) { _sensors.Add(sensor); }

		public RegisteredSensorTypeOfInterest()
		{
			_sensors = new List<WatchedSensor>();
		}

		public RegisteredSensorTypeOfInterest(RegisteredSensorTypeOfInterest source)
		{
			_sensors = source._sensors.ToList();
		}

		public RegisteredSensorTypeOfInterest Copy()
		{
			return new RegisteredSensorTypeOfInterest(this);
		}

		private WatchedSensor SensorOfInterest(string name)
		{
			foreach (WatchedSensor sensor in _sensors)
			{
				if (string.IsNullOrWhiteSpace(name) || name.Contains(sensor.Name))
				{
					return sensor;
				}
			}

			return null;
		}
	}

	class RegisteredSensorsOfInterest
	{
		private RegisteredSensorTypeOfInterest[] _sensors = new RegisteredSensorTypeOfInterest[(int)SensorType.Count];

		public RegisteredSensorTypeOfInterest this[SensorType sensorType] => _sensors[(int)sensorType];

		public bool IsEmpty
		{
			get
			{
				foreach (RegisteredSensorTypeOfInterest sensorType in _sensors)
				{
					if (!sensorType.IsEmpty)
						return false;
				}
				return true;
			}
		}

		public RegisteredSensorsOfInterest()
		{
			for (int i = 0; i < (int)SensorType.Count; ++i)
			{
				_sensors[i] = new RegisteredSensorTypeOfInterest();
			}
		}

		private RegisteredSensorsOfInterest(RegisteredSensorsOfInterest source)
		{
			for (int i = 0; i < (int)SensorType.Count; ++i)
			{
				_sensors[i] = source._sensors[i].Copy();
			}
		}

		public RegisteredSensorsOfInterest Copy()
		{
			return new RegisteredSensorsOfInterest(this);
		}
	}

	class RegisteredHardwareOfInterest
	{
		private RegisteredSensorsOfInterest[] _hardware = new RegisteredSensorsOfInterest[(int)HardwareType.Count];
		private object _mutex = new object();

		public RegisteredSensorsOfInterest this[HardwareType hardwareType] => _hardware[(int)hardwareType];

		public void Add(WatchedSensor sensorOfInterest)
		{
			lock (_mutex)
			{
				this[sensorOfInterest.HardwareType][sensorOfInterest.SensorType].Add(sensorOfInterest);
			}
		}

		public RegisteredHardwareOfInterest()
		{
			for (int i = 0; i < (int)HardwareType.Count; ++i)
			{
				_hardware[i] = new RegisteredSensorsOfInterest();
			}
		}

		private RegisteredHardwareOfInterest(RegisteredHardwareOfInterest source)
		{
			lock (_mutex)
			{
				for (int i = 0; i < (int)HardwareType.Count; ++i)
				{
					_hardware[i] = source._hardware[i].Copy();
				}
			}
		}

		public RegisteredHardwareOfInterest Copy()
		{
			return new RegisteredHardwareOfInterest(this);
		}
	}

	struct HardwareData
	{
		public HardwareType HardwareType { get; set; }

		public string Name { get; set; }
	}

	struct SensorData
	{
		public WatchedSensor SensorOfInterest { get; set; }
		public string Name { get; set; }
		public float Value { get; set; }
	}

	class Monitor
	{
		private Thread _thread;
		private volatile bool _keepPolling = true;
		private RegisteredHardwareOfInterest _registeredHardwareOfInterest = new RegisteredHardwareOfInterest();

		public delegate void RegisteredSensorPolledHandler(SensorData sensorData);
		public event RegisteredSensorPolledHandler RegisteredSensorPolled;

		public delegate void RegisteredHardwarePolledHandler(HardwareData hardwareData);
		public event RegisteredHardwarePolledHandler RegisteredHardwarePolled;

		public delegate void SensorPollingHandler();
		public event SensorPollingHandler SensorPollingStarting;
		public event SensorPollingHandler SensorPollingFinished;

		public void StartPolling()
		{
			_thread = new Thread(new ThreadStart(this.ThreadStart));

			_keepPolling = true;
			_thread.Start();
		}

		public void StopPolling(bool wait = false)
		{
			_keepPolling = false;

			if (wait)
			{
				_thread.Join();
			}
		}

		public void RegisterSensorOfInterest(WatchedSensor sensorOfInterest)
		{
			_registeredHardwareOfInterest.Add(sensorOfInterest);
		}

		private void ThreadStart()
		{
			while (_keepPolling)
			{
				PollHardware();

				Thread.Sleep(1000);
			}
		}

		private void PollHardware()
		{
			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { SensorPollingStarting?.Invoke(); }));

			RegisteredHardwareOfInterest hardwareOfInterest = _registeredHardwareOfInterest.Copy();

			Computer computer = new Computer
			{
				IsCpuEnabled = true,
				IsGpuEnabled = true,
				IsMemoryEnabled = true,
				IsMotherboardEnabled = true,
				IsControllerEnabled = true,
				IsNetworkEnabled = true,
				IsStorageEnabled = true
			};

			computer.Open();
			computer.Accept(new UpdateVisitor());

			PollHardware(hardwareOfInterest, computer.Hardware);
			//DebugPrintHardwareToConsole(hardwareOfInterest, computer);

			computer.Close();

			Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { SensorPollingFinished?.Invoke(); }));
		}

		private void DebugPrintHardwareToConsole(RegisteredHardwareOfInterest hardwareOfInterest, Computer computer)
		{
			foreach (IHardware hardware in computer.Hardware)
			{
				Debug.WriteLine($"Hardware: {hardware.Name}, type: {hardware.HardwareType}");

				foreach (IHardware subhardware in hardware.SubHardware)
				{
					Debug.WriteLine($"\tSubhardware: {subhardware.Name}, type: {subhardware.HardwareType}");

					foreach (ISensor sensor in subhardware.Sensors)
					{
						Debug.WriteLine($"\t\tSensor: {sensor.Name}, value: {sensor.Value}, type: {sensor.SensorType}");
					}
				}

				foreach (ISensor sensor in hardware.Sensors)
				{
					Debug.WriteLine($"\t\tSensor: {sensor.Name}, value: {sensor.Value}, type: {sensor.SensorType}");
				}
			}
		}

		private static HardwareType Convert(LibreHardwareMonitor.Hardware.HardwareType hardwareType)
		{
			switch (hardwareType)
			{
				case LibreHardwareMonitor.Hardware.HardwareType.Cpu:
					return HardwareType.Cpu;

				case LibreHardwareMonitor.Hardware.HardwareType.GpuAmd:
				case LibreHardwareMonitor.Hardware.HardwareType.GpuNvidia:
					return HardwareType.Gpu;

				case LibreHardwareMonitor.Hardware.HardwareType.Memory:
					return HardwareType.Memory;
			}

			return HardwareType.Ignored;
		}

		private static SensorType Convert(LibreHardwareMonitor.Hardware.SensorType sensorType)
		{
			switch (sensorType)
			{
				case LibreHardwareMonitor.Hardware.SensorType.Clock:
					return SensorType.Clock;

				case LibreHardwareMonitor.Hardware.SensorType.Load:
					return SensorType.Load;

				case LibreHardwareMonitor.Hardware.SensorType.Temperature:
					return SensorType.Temperature;

				case LibreHardwareMonitor.Hardware.SensorType.Data:
					return SensorType.Data;
			}

			return SensorType.Ignored;
		}

		bool IsHardwareOfInterest(RegisteredHardwareOfInterest hardwareOfInterest, IHardware hardware)
		{
			HardwareType hardwareType = Convert(hardware.HardwareType);

			if (hardwareType == HardwareType.Ignored)
				return false;

			return !hardwareOfInterest[hardwareType].IsEmpty;
		}

		bool IsSensorOfInterest(RegisteredHardwareOfInterest hardwareOfInterest, ISensor sensor)
		{
			SensorType sensorType = Convert(sensor.SensorType);

			return sensorType != SensorType.Ignored;
		}

		private void PollHardware(RegisteredHardwareOfInterest hardwareOfInterest, IEnumerable<IHardware> hardware)
		{
			foreach (IHardware hardwareItem in hardware)
			{
				if (!IsHardwareOfInterest(hardwareOfInterest, hardwareItem))
					continue;

				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { RegisteredHardwarePolled?.Invoke(new HardwareData() { Name = hardwareItem.Name, HardwareType = Convert(hardwareItem.HardwareType) }); }));

				PollSensors(hardwareOfInterest, hardwareItem);
			}
		}

		private void PollSensors(RegisteredHardwareOfInterest hardwareOfInterest, IHardware hardware)
		{
			PollHardware(hardwareOfInterest, hardware.SubHardware);

			foreach (ISensor sensor in hardware.Sensors)
			{
				if (!IsSensorOfInterest(hardwareOfInterest, sensor))
					continue;

				WatchedSensor sensorOfInterest = hardwareOfInterest[Convert(sensor.Hardware.HardwareType)][Convert(sensor.SensorType)][sensor.Name];

				if (sensorOfInterest == null)
					continue;

				Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { RegisteredSensorPolled?.Invoke(new SensorData() { SensorOfInterest = sensorOfInterest, Name = sensor.Name, Value = sensor.Value ?? 0.0f }); }));
			}
		}
	}
}
//*/
