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

			string appdataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

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
