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

namespace HardwareMonitor.Monitor
{
	/// <summary>
	/// Contains the values obtained from a single poll of the hardware.
	/// </summary>
	public class SensorSample
	{
		#region Public Properties

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

		#endregion Public Properties
	}

	/// <summary>
	/// Contains any useful information we want about the hardware itself. Typically this information is unchanged from poll to poll
	/// </summary>
	public class HardwareSample
	{
		#region Public Properties

		public Config.Component Component { get; set; }

		public string Name { get; set; }

		#endregion Public Properties
	}

	/// <summary>
	/// represents a piece of information we're interested in *captured* with a custom name defined by the data
	/// </summary>
	public class Capture
	{
		#region Public Properties

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

		#endregion Public Properties
	}

	/// <summary>
	/// Contains all the data sampled and captured during a poll of the hardware and sensors
	/// </summary>
	public class Snapshot
	{
		#region Private fields

		private List<HardwareSample> _hardwareSamples = new List<HardwareSample>();
		private List<SensorSample> _sensorSamples = new List<SensorSample>();
		private Dictionary<string, Capture> _captures = new Dictionary<string, Capture>();

		#endregion Private fields

		#region Public Properties

		public List<HardwareSample> HardwareSamples => _hardwareSamples;
		public List<SensorSample> SensorSamples => _sensorSamples;
		public Dictionary<string, Capture> Captures => _captures;
		
		#endregion Public Properties
	}
}
