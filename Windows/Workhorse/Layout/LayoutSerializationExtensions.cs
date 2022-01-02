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

using System.Linq;

namespace HardwareMonitor.Layout
{
	public class CollectedValues
	{
		#region Private Fields

		private dynamic[] _values = new dynamic[512];
		private int _length = 0;

		#endregion Private Fields

		#region Public Properties

		//@todo: Refactor to use ArraySegment (to reduce memory allocations)
		//public dynamic[] Values => new ArraySegment<dynamic>(_values, 0, Length);
		public dynamic[] Values => _values.Take(_length).ToArray();

		public int Length => _length;

		#endregion Public Properties

		#region Public Methods

		public void Add(dynamic value)
		{
			_values[_length] = value;
			++_length;
		}

		#endregion Public Methods
	}

	partial class Config
	{
		#region Public Methods

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

		#endregion Public Methods
	}

	partial class Module
	{
		#region Public Methods

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

		#endregion Public Methods
	}

	abstract partial class Component
	{
		#region Abstract Properties
		
		public abstract byte ConstructionId { get; }

		#endregion Abstract Properties

		#region Abstract Methods

		public abstract void CollectValues(CollectedValues collectedValues);

		#endregion Abstract Methods

		#region Public Methods

		public void CollectBaseValues(CollectedValues collectedValues)
		{
			collectedValues.Add(X);
			collectedValues.Add(Y);
		}

		#endregion Public Methods
	}

	partial class Border
	{
		#region Component

		public override byte ConstructionId => 0;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(Width);
			collectedValues.Add(Height);
			collectedValues.Add((ushort)Colour);
		}

		#endregion Component
	}

	partial class Text
	{
		#region Component

		public override byte ConstructionId => 1;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(TextSize);
		}

		#endregion Component
	}

	partial class Metric
	{
		#region Component

		public override byte ConstructionId => 2;

		public override void CollectValues(CollectedValues collectedValues)
		{
			collectedValues.Add(TextSize);
			collectedValues.Add(UnitTextSize);
			collectedValues.Add(Unit);
			collectedValues.Add(Precision);
		}

		#endregion Component
	}

	partial class Icon
	{
		#region Component

		public override byte ConstructionId => 3;

		public override void CollectValues(CollectedValues collectedValues)
		{
		}

		#endregion Component
	}
}
