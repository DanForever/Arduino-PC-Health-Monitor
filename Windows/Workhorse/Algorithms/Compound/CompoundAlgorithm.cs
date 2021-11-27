using System.Collections.Generic;

namespace HardwareMonitor.Algorithms.Compound
{
	public interface IAlgorithm
	{
		float Calculate(IEnumerable<float> values);
	}
}
