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
