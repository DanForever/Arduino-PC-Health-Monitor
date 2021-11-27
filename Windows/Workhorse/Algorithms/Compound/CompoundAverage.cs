using System.Collections.Generic;

namespace HardwareMonitor.Algorithms.Compound
{
	public class Average : IAlgorithm
	{
		#region ICompoundAlgorithm
		
		float IAlgorithm.Calculate(IEnumerable<float> values)
		{
			int count = 0;
			float sum = 0;

			foreach(float value in values)
			{
				sum += value;
				++count;
			}

			float average = sum / count;

			return average;
		}

		#endregion ICompoundAlgorithm
	}
}
