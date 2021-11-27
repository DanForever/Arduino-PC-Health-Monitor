using System;
using System.Collections.Generic;

using HardwareMonitor;
using HardwareMonitor.Plugin;

namespace AdditionalMemoryMetrics
{
	namespace Hardware
	{
		internal class Memory : IHardware
		{
			private string _name;

			public Memory(string name)
			{
				_name = name;
			}

			string IHardware.Name => _name;

			HardwareType IHardware.Type => HardwareType.Memory;

			IEnumerable<ISensor> IHardware.Sensors => Array.Empty<ISensor>();
		}
	}
}
