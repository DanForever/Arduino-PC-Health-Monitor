namespace HardwareMonitor
{
	public enum HardwareType
	{
		Cpu,
		Gpu,
		Memory,
		Motherboard,
		Network,
		Storage,

		Count,
		Ignored,
	}

	public enum SensorType
	{
		Clock,
		Load,
		Temperature,

		Data,

		Count,
		Ignored,
	}
}
