using System.Collections.Generic;

namespace HardwareMonitor.Plugin
{
	public interface ISource
	{
		public IEnumerable<IHardware> Hardware { get; }

		public void Update();
	}
}
