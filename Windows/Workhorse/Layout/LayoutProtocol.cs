using System.Collections.Generic;
using System.Xml.Serialization;

namespace HardwareMonitor.Layout
{
	public partial class Component
	{
		[XmlAttribute]
		public string UseCapture { get; set; }

		[XmlAttribute]
		public bool NoUpdate { get; set; }
	}

	public class MappedComponent
	{
		public Component Component { get; set; }
		public byte ModuleIndex { get; set; }
		public byte ComponentIndex { get; set; }
	}

	public partial class Config
	{
		private Dictionary<string, List<MappedComponent>> _mappedComponents = new Dictionary<string, List<MappedComponent>>();

		[XmlIgnore]
		public Dictionary<string, List<MappedComponent>> MappedComponents => _mappedComponents;

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
	}
}
