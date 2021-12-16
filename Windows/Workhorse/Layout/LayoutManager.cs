using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareMonitor.Layout
{
	public class LayoutManager
	{
		private List<Config> _layouts = new List<Config>();

		public List<Config> Layouts => _layouts;

		public Config GetLayout(eResolution resolution, Orientation orientation)
		{
			foreach (Config config in _layouts)
			{
				if (config.Resolution == resolution && config.Orientation == orientation)
				{
					return config;
				}
			}

			return null;
		}

		public void Load()
		{
			string[] defaultConfigs;
			try
			{
				defaultConfigs = Directory.GetFiles("Data/Layouts/");
			}
			catch(System.IO.DirectoryNotFoundException)
			{
				//@todo: global feedback class, not a feedback member of main
				defaultConfigs = new string[0];
			}

			Load(defaultConfigs);

			string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			// @todo: User layouts
		}

		private void Load(string[] configs)
		{
			foreach (string configPath in configs)
			{
				Config layout = Config.Load(configPath);

				Layouts.Add(layout);
			}
		}
	}
}
