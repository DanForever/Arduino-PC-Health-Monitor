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
using System.Runtime.Versioning;

using HardwareMonitor;
using HardwareMonitor.Plugin;

using Libre = LibreHardwareMonitor.Hardware;

[assembly: UnsupportedOSPlatform("ios")]
[assembly: UnsupportedOSPlatform("android")]
namespace LibreHardwareMetrics
{
	internal static class LibreExtensions
	{
		#region Mapping Libre types to local types

		// These are the various hardware and sensor types that can be polled in libre hardware monitor
		// We map them to our own types so that we're not coupled to libre
		// The ones that are commented out simply haven't really been tested, there's nothing stopping them from being added
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
			// If we fail to find a mapped type, simply return the "Ignored" type
			return _hardwareTypeMap.GetValueOrDefault(hardwareType, HardwareType.Ignored);
		}

		public static SensorType Convert(this Libre.SensorType sensorType)
		{
			return _sensorTypeMap.GetValueOrDefault(sensorType, SensorType.Ignored);
		}

		#endregion Mapping Libre types to local types
	}

	class Sensor : ISensor
	{
		#region Private Fields

		private Libre.ISensor _libreSensor;
		private Hardware _parent;

		#endregion Private Fields

		#region C-Tor

		public Sensor(Libre.ISensor libreSensor, Hardware parent)
		{
			_libreSensor = libreSensor;
			_parent = parent;
		}

		#endregion C-Tor

		#region ISensor

		string ISensor.Name => _libreSensor.Name;

		float ISensor.Value => _libreSensor.Value ?? 0.0f;

		SensorType ISensor.Type => _libreSensor.SensorType.Convert();

		IHardware ISensor.Hardware => _parent;

		#endregion ISensor
	}

	class Hardware : IHardware
	{
		#region Private Fields

		private Libre.IHardware _libreHardware;

		#endregion Private Fields

		#region C-Tor

		public Hardware(Libre.IHardware libreHardware)
		{
			_libreHardware = libreHardware;
		}

		#endregion C-Tor

		#region Iterators

		internal static IEnumerable<IHardware> HardwareIterator(IEnumerable<Libre.IHardware> hardware)
		{
			foreach (Libre.IHardware hardwareItem in hardware)
			{
				yield return new Hardware(hardwareItem);
			}
		}

		internal static IEnumerable<ISensor> SensorIterator(IEnumerable<Libre.ISensor> sensors, Hardware parent)
		{
			foreach (Libre.ISensor libreSensor in sensors)
			{
				yield return new Sensor(libreSensor, parent);
			}
		}

		#endregion Iterators

		#region IHardware

		string IHardware.Name => _libreHardware.Name;
		HardwareType IHardware.Type => _libreHardware.HardwareType.Convert();
		IEnumerable<IHardware> IHardware.Hardware => HardwareIterator(_libreHardware.SubHardware);
		IEnumerable<ISensor> IHardware.Sensors => SensorIterator(_libreHardware.Sensors, this);

		#endregion IHardware
	}

	class Source : ISource
	{
		#region Private Fields

		private Libre.Computer _computer;

		#endregion Private Fields

		#region ISource

		IEnumerable<IHardware> ISource.Hardware => Hardware.HardwareIterator(_computer.Hardware);

		void ISource.PollingStarted(HardwareMonitor.Monitor.Config.Computer config)
		{
			_computer = new Libre.Computer
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

			_computer.Open();
			_computer.Accept(new Visitor());
		}

		void ISource.PollingFinished(HardwareMonitor.Monitor.Config.Computer config)
		{
			_computer.Close();
			_computer = null;
		}

		#endregion ISource
	}
}
