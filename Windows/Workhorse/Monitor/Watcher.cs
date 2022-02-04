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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareMonitor.Monitor.Asynchronous
{
	/// <summary>
	/// Public API that presents functions that can be called using async / await
	/// </summary>
	public static class Watcher
	{
		#region Public Methods

		public static async Task<Snapshot> Poll(Config.Computer config, IEnumerable<Plugin.ISource> sources)
		{
			return await Task.Run(() => Synchronous.Watcher.Poll(config, sources));
		}
		
		#endregion Public Methods
	}
}

namespace HardwareMonitor.Monitor.Synchronous
{
	/// <summary>
	/// Public API that presents functions that will execute synchronously
	/// </summary>
	public static class Watcher
	{
		#region Private Classes

		private class CompoundSensorData
		{
			#region Private fields

			private List<SensorSample> _samples = new List<SensorSample>();

			#endregion Private fields

			#region Public Properties

			public List<SensorSample> Samples => _samples;

			public Config.CompoundSensor Sensor { get; set; }
			
			#endregion Public Properties
		}

		private class PollData
		{
			#region Public Properties

			public Config.Computer Config { get; set; }
			public IEnumerable<Plugin.ISource> Sources { get; set; }
			public Dictionary<string, CompoundSensorData> CompoundSensors { get; set; }
			public Snapshot Snapshot { get; set; }

			#endregion Public Properties
		}

		#endregion Private Classes

		#region Private fields

		private static PollData _data = new PollData();

		#endregion Private fields

		#region Public Methods

		public static Snapshot Poll(Config.Computer config, IEnumerable<Plugin.ISource> sources)
		{
			_data.Config = config;
			_data.Sources = sources;
			_data.CompoundSensors = new Dictionary<string, CompoundSensorData>();
			_data.Snapshot = new Snapshot();

			PollPlugins(_data);
			ProcessCompoundSensors(_data);

			return _data.Snapshot;
		}

		public static void Dump(StreamWriter stream, IEnumerable<Plugin.ISource> sources)
		{
			Config.DummyComputer dummyConfig = new Config.DummyComputer();

			foreach (Plugin.ISource source in sources)
			{
				source.PollingStarted(dummyConfig);
					DumpHardware(stream, source.Hardware);
				source.PollingFinished(dummyConfig);
			}
		}

		#endregion Public Methods

		#region Polling

		private static void PollPlugins(PollData data)
		{
			foreach (Plugin.ISource source in data.Sources)
			{
				source.PollingStarted(data.Config);
					PollHardware(source.Hardware, data);
				source.PollingFinished(data.Config);
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
			PollHardware(hardware.Hardware, data);

			foreach (Plugin.ISensor sensor in hardware.Sensors)
			{
				CheckSensor(sensor, data);
				CheckCompoundSensor(sensor, data);
			}
		}

		private static void CheckSensor(Plugin.ISensor sensor, PollData data)
		{
			Config.Sensor watchedSensor = data.Config.FindSensor(sensor.Name, sensor.Hardware.Type, sensor.Type);
			if (watchedSensor == null)
				return;

			data.Snapshot.SensorSamples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value });

			if (!string.IsNullOrWhiteSpace(watchedSensor.CaptureName))
				data.Snapshot.Captures.Add(watchedSensor.CaptureName, new Capture() { Name = watchedSensor.CaptureName, Value = sensor.Value });
		}

		private static void CheckCompoundSensor(Plugin.ISensor sensor, PollData data)
		{
			Config.CompoundSensor watchedCompoundSensor = data.Config.FindCompoundSensor(sensor.Name, sensor.Hardware.Type, sensor.Type);
			if (watchedCompoundSensor == null)
				return;

			CompoundSensorData compoundSensorData;
			if (!data.CompoundSensors.TryGetValue(watchedCompoundSensor.Name, out compoundSensorData))
			{
				compoundSensorData = new CompoundSensorData() { Sensor = watchedCompoundSensor };
				data.CompoundSensors.Add(watchedCompoundSensor.Name, compoundSensorData);
			}

			Config.Sensor watchedSensor = data.Config.FindSensor(watchedCompoundSensor.Sensors, sensor.Name, sensor.Hardware.Type, sensor.Type);
			compoundSensorData.Samples.Add(new SensorSample() { Sensor = watchedSensor, Name = sensor.Name, Value = sensor.Value });
		}


		#endregion Polling

		#region Dump

		private static void DumpHardware(StreamWriter stream, IEnumerable<Plugin.IHardware> hardware, string prefix = "")
		{
			foreach (Plugin.IHardware hardwareItem in hardware)
			{
				stream.WriteLine($"{prefix}Hardware:");
				stream.WriteLine($"{prefix} - Name: {hardwareItem.Name}");
				stream.WriteLine($"{prefix} - Type: {hardwareItem.Type}");

				DumpHardware(stream, hardwareItem.Hardware, $"{prefix}  ");
				DumpSensors(stream, hardwareItem.Sensors, $"{prefix}  ");
			}
		}


		private static void DumpSensors(StreamWriter stream, IEnumerable<Plugin.ISensor> sensors, string prefix)
		{
			foreach (Plugin.ISensor sensor in sensors)
			{
				stream.WriteLine($"{prefix}Sensor:");
				stream.WriteLine($"{prefix} - Name: {sensor.Name}");
				stream.WriteLine($"{prefix} - Type: {sensor.Type}");
				stream.WriteLine($"{prefix} -Value: {sensor.Value}");
			}
		}

		#endregion Dump

		#region Private Methods

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

		#endregion Private Methods
	}
}
