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
using System.Xml.Serialization;

namespace HardwareMonitor.Layout
{
	public partial class Component
	{
		#region Public Properties

		[XmlAttribute]
		public string UseCapture { get; set; }

		[XmlAttribute]
		public bool NoUpdate { get; set; }

		#endregion Public Properties
	}

	public class MappedComponent
	{
		#region Public Properties

		public Component Component { get; set; }
		public byte ModuleIndex { get; set; }
		public byte ComponentIndex { get; set; }

		#endregion Public Properties
	}

	public partial class Config
	{
		#region Private Methods

		private Dictionary<string, List<MappedComponent>> _mappedComponents = new Dictionary<string, List<MappedComponent>>();

		#endregion Private Methods

		#region Public Properties

		[XmlIgnore]
		public Dictionary<string, List<MappedComponent>> MappedComponents => _mappedComponents;

		#endregion Public Properties

		#region ConfigBase

		protected override void OnLoadFinished()
		{
			base.OnLoadFinished();

			for(byte iModule = 0; iModule < Modules.Count; ++iModule)
			{
				Module module = Modules[iModule];

				for (byte iComponent = 0; iComponent < module.Components.Count; ++iComponent)
				{
					Component component = module.Components[iComponent];

					if(!string.IsNullOrWhiteSpace(component.UseCapture))
					{
						MappedComponent mappedComponent = new MappedComponent()
						{
							Component = component,
							ModuleIndex = iModule,
							ComponentIndex = iComponent,
						};

						List<MappedComponent> mappedToCapture;
						if(!_mappedComponents.TryGetValue(component.UseCapture, out mappedToCapture))
						{
							mappedToCapture = new List<MappedComponent>();
							_mappedComponents.Add(component.UseCapture, mappedToCapture);
						}

						mappedToCapture.Add(mappedComponent);
					}
				}
			}
		}

		#endregion ConfigBase
	}
}
