using System.Collections.Generic;
using System.Management;

using AdditionalMemoryMetrics.Hardware;

using HardwareMonitor.Plugin;

namespace AdditionalMemoryMetrics
{
	public class Source : ISource
	{
		private Memory[] _hardware = new Memory[1];

		IEnumerable<IHardware> ISource.Hardware => _hardware;

		void ISource.Update()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
			foreach (ManagementObject obj in searcher.Get())
			{
				string manufacturer = (string)obj.Properties["Manufacturer"].Value;
				string productCode = (string)obj.Properties["PartNumber"].Value;

				_hardware[0] = new Memory(productCode);

				// We only care about the first result we find
				// If there are non-matched ram sticks in this machine ¯\_(ツ)_/¯
				break;
			}
		}
	}
}
