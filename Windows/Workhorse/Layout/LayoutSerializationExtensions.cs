using System.Linq;

namespace HardwareMonitor.Layout
{
	public class CollectedValues
	{
		private dynamic[] _values = new dynamic[512];
		private int _length = 0;

		//public dynamic[] Values => new ArraySegment<dynamic>(_values, 0, Length);
		public dynamic[] Values => _values.Take(_length).ToArray();

		public int Length => _length;

		public void Add(dynamic value)
		{
			_values[_length] = value;
			++_length;
		}
	}

	partial class Config
	{
		public dynamic[] CollectValues()
		{
			CollectedValues collectedValues = new CollectedValues();

			collectedValues.Add(Protocol.PacketType.ModuleDefinition);
			collectedValues.Add((byte)Modules.Count);

			foreach(Module module in Modules)
			{
				module.CollectValues(collectedValues);
			}

			return collectedValues.Values;
		}
	}

	partial class Module
	{
		public void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(X);
			collectedValues.Add(Y);
			collectedValues.Add((byte)Components.Count);

			for(byte i = 0; i < Components.Count; ++i)
			{
				collectedValues.Add((byte)0); // Component definition
				collectedValues.Add(Components[i].ConstructionId);
				Components[i].CollectBaseValues(collectedValues);
				Components[i].CollectValues(collectedValues);
			}
		}
	}

	abstract partial class Component
	{
		public abstract byte ConstructionId { get; }

		public void CollectBaseValues(CollectedValues collectedValues)
		{
			collectedValues.Add(X);
			collectedValues.Add(Y);
		}

		public abstract void CollectValues(CollectedValues collectedValues);
	}

	partial class Border
	{
		public override byte ConstructionId => 0;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(Width);
			collectedValues.Add(Height);
			collectedValues.Add((ushort)Colour);
		}
	}

	partial class Text
	{
		public override byte ConstructionId => 1;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(TextSize);
		}
	}

	partial class Metric
	{
		public override byte ConstructionId => 2;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(TextSize);
			collectedValues.Add(UnitTextSize);
			collectedValues.Add(Unit);
			collectedValues.Add(Precision);
		}
	}

	partial class Icon
	{
		public override byte ConstructionId => 3;

		public override void CollectValues(CollectedValues collectedValues)
		{
		}
	}
}
