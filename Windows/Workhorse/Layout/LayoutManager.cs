using System;
using System.Collections.Generic;
using System.IO;

namespace HardwareMonitor.Layout
{
	public class LayoutManager
	{
		#region Private Fields

		private List<Config> _layouts = new List<Config>();

		#endregion Private Fields

		#region Public Properties

		public List<Config> Layouts => _layouts;

		#endregion Public Properties

		#region Public Methods

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

		#endregion Public Methods

		#region Private Methods

		private void Load(string[] configs)
		{
			foreach (string configPath in configs)
			{
				Config layout = Config.Load(configPath);

				Layouts.Add(layout);
			}
		}

		#endregion Private Methods
	}
}
