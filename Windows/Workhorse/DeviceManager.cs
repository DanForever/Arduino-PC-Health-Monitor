using System;
/*
namespace HardwareMonitor.Device
{
	enum MessageCategory
	{
		SensorUpdate,
		Guaranteed,
		GuaranteedAck,
	}

	enum Metrics
	{
		CpuName,
		CpuIcon,
		CpuAverageClock,
		CpuTemperature,
		CpuTotalLoad,

		GpuName,
		GpuIcon,
		GpuClock,
		GpuTemperature,
		GpuLoad,

		MemoryName,
		MemoryIcon,
		MemoryTotal,
		MemoryUsage,
	}

	class DeviceManager
	{
		private Connection.Manager _connectionManager;

		private float _averageCoreClock = 0;
		private float _numCores = 0;

		private float _memoryAvailable = 0;
		private float _memoryUsed = 0;

		// Things like names of hardware etc
		private bool[] _sendStaticInformation = new bool[(int)HardwareType.Count];

		public void Initialize(Monitor.Monitor hardwareMonitor, Connection.Manager connectionManager)
		{
			_connectionManager = connectionManager;

			_connectionManager.NewActiveConnection += _connectionManager_NewActiveConnection;

			hardwareMonitor.RegisteredHardwarePolled += HardwareMonitor_RegisteredHardwarePolled;
			hardwareMonitor.RegisteredSensorPolled += HardwareMonitor_RegisteredSensorPolled;
			hardwareMonitor.SensorPollingStarting += HardwareMonitor_SensorPollingStarting;
			hardwareMonitor.SensorPollingFinished += HardwareMonitor_SensorPollingFinished;

			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Cpu, SensorType = SensorType.Temperature, Name = "CPU Package" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Cpu, SensorType = SensorType.Temperature, Name = "Core (Tctl/Tdie)" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Cpu, SensorType = SensorType.Clock, Name = "Core #" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Cpu, SensorType = SensorType.Load, Name = "CPU Total" });

			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Gpu, SensorType = SensorType.Temperature, Name = "GPU Core" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Gpu, SensorType = SensorType.Clock, Name = "GPU Core" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Gpu, SensorType = SensorType.Load, Name = "GPU Core" });

			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Memory, SensorType = SensorType.Data, Name = "Memory Used" });
			hardwareMonitor.RegisterSensorOfInterest(new SensorOfInterest() { HardwareType = HardwareType.Memory, SensorType = SensorType.Data, Name = "Memory Available" });
		}

		private void CpuHandler(Monitor.SensorData sensorData)
		{
			switch (sensorData.SensorOfInterest.SensorType)
			{
				case SensorType.Clock:
					CpuClockHandler(sensorData);
					break;

				case SensorType.Temperature:
					if (sensorData.Name == "Core (Tctl/Tdie)")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.CpuTemperature, sensorData.Value);
					else if (sensorData.Name == "CPU Package")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.CpuTemperature, sensorData.Value);
					break;

				case SensorType.Load:
					if (sensorData.Name == "CPU Total")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.CpuTotalLoad, sensorData.Value);
					break;
			}
		}

		private void CpuClockHandler(Monitor.SensorData sensorData)
		{
			if (sensorData.Name.Contains("Core #"))
			{
				_averageCoreClock += sensorData.Value;
				++_numCores;
			}
		}

		private void SendAverageClock()
		{
			if (_numCores > 0)
			{
				_averageCoreClock /= _numCores;
				_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.CpuAverageClock, _averageCoreClock);
			}
		}

		private void SendTotalMemory()
		{
			float totalMemory = MathF.Ceiling(_memoryUsed + _memoryAvailable);
			_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.MemoryTotal, totalMemory);
		}

		private void GpuHandler(Monitor.SensorData sensorData)
		{
			switch (sensorData.SensorOfInterest.SensorType)
			{
				case SensorType.Clock:
					if (sensorData.Name == "GPU Core")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.GpuClock, sensorData.Value);
					break;
				case SensorType.Temperature:
					if (sensorData.Name == "GPU Core")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.GpuTemperature, sensorData.Value);
					break;
				case SensorType.Load:
					if (sensorData.Name == "GPU Core")
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.GpuLoad, sensorData.Value);
					break;
			}
		}

		private void MemoryHandler(Monitor.SensorData sensorData)
		{
			switch (sensorData.SensorOfInterest.SensorType)
			{
				case SensorType.Data:
					if (sensorData.Name == "Memory Used")
					{
						_connectionManager.Send(MessageCategory.SensorUpdate, Metrics.MemoryUsage, sensorData.Value);
						_memoryUsed = sensorData.Value;
					}
					else if (sensorData.Name == "Memory Available")
					{
						_memoryAvailable = sensorData.Value;
					}
					break;
			}
		}

		private void HardwareMonitor_SensorPollingStarting()
		{
			_averageCoreClock = 0.0f;
			_numCores = 0;
		}

		private void HardwareMonitor_SensorPollingFinished()
		{
			SendAverageClock();
			SendTotalMemory();
		}

		private void HardwareMonitor_RegisteredSensorPolled(Monitor.SensorData sensorData)
		{
			switch (sensorData.SensorOfInterest.HardwareType)
			{
				case HardwareType.Cpu:
					CpuHandler(sensorData);
					break;

				case HardwareType.Gpu:
					GpuHandler(sensorData);
					break;

				case HardwareType.Memory:
					MemoryHandler(sensorData);
					break;
			}
		}

		private void SendIconForHardware(Metrics metric, string hardwareName)
		{
			string filepath = null;
			if (hardwareName.Contains("Ryzen"))
				filepath = "Images/ryzen_black.bmp";
			else if (hardwareName.Contains("Intel Core i7-8"))
				filepath = "Images/intel_gen8_i7.bmp";
			else if (hardwareName.Contains("RTX"))
				filepath = "Images/nvidia_rtx.bmp";
			else if (hardwareName.Contains("GTX"))
				filepath = "Images/nvidia_gtx.bmp";
			else if (hardwareName.Contains("TZN"))
				filepath = "Images/gskill_tzn.bmp";

			if (!string.IsNullOrWhiteSpace(filepath))
			{
				Connection.IconSender.Send(metric, filepath, _connectionManager);
			}
		}

		private void HardwareMonitor_RegisteredHardwarePolled(Monitor.HardwareData hardwareData)
		{
			if (_sendStaticInformation[(int)hardwareData.HardwareType])
			{
				_sendStaticInformation[(int)hardwareData.HardwareType] = false;

				switch (hardwareData.HardwareType)
				{
					case HardwareType.Cpu:
						_connectionManager.SendGuaranteed(MessageCategory.SensorUpdate, Metrics.CpuName, hardwareData.Name);
						SendIconForHardware(Metrics.CpuIcon, hardwareData.Name);
						break;

					case HardwareType.Gpu:
						_connectionManager.SendGuaranteed(MessageCategory.SensorUpdate, Metrics.GpuName, hardwareData.Name);
						SendIconForHardware(Metrics.GpuIcon, hardwareData.Name);
						break;
				}
			}
		}

		private void SendMemoryInfo()
		{
			_connectionManager.SendGuaranteed(MessageCategory.SensorUpdate, Metrics.MemoryName, AdditionalHardwareStats.Memory.ProductCode);
			SendIconForHardware(Metrics.MemoryIcon, AdditionalHardwareStats.Memory.ProductCode);
		}

		private void _connectionManager_NewActiveConnection(ActiveConnection connection)
		{
			Array.Fill(_sendStaticInformation, true);

			SendMemoryInfo();
		}
	}
}
//*/
