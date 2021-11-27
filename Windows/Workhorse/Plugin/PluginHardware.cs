using System.Collections.Generic;

namespace HardwareMonitor.Plugin
{
	public interface IHardware
	{
		public string Name { get; }

		public HardwareType Type { get; }

		public IEnumerable<ISensor> Sensors { get; }
	}
}
