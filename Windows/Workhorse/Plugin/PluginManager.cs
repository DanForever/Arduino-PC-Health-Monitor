using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HardwareMonitor.Plugin
{
	public class Manager
	{
		private List<ISource> _sources;

		public IEnumerable<ISource> Sources => _sources;

		public Manager(Config.Config config)
		{
			string[] pluginPaths = BuildPluginPaths(config);

			_sources = pluginPaths.SelectMany(pluginPath =>
			{
				Assembly pluginAssembly = LoadPlugin(pluginPath);
				return CreateSources(pluginAssembly);
			}).ToList();
		}

		private string[] BuildPluginPaths(Config.Config config)
		{
			string[] paths = new string[config.Plugins.Count];

			for(int i = 0; i < paths.Length; ++i)
			{
				var pluginConfig = config.Plugins[i];

#if DEBUG
				// Navigate up to the solution root
				string root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(typeof(Manager).Assembly.Location))));

				paths[i] = Path.GetFullPath(Path.Combine(root, pluginConfig.Name, pluginConfig.Name + ".dll"));
#else
				paths[i] = Path.Combine("Plugins", pluginConfig.Name) + ".dll";
#endif
			}

			return paths;
		}

		static Assembly LoadPlugin(string absolutePath)
		{
			Debug.WriteLine($"Loading commands from: {absolutePath}");
			LoadContext loadContext = new LoadContext(absolutePath);
			return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(absolutePath));
		}

		static IEnumerable<ISource> CreateSources(Assembly assembly)
		{
			int count = 0;

			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(ISource).IsAssignableFrom(type))
				{
					ISource result = Activator.CreateInstance(type) as ISource;
					if (result != null)
					{
						++count;
						yield return result;
					}
				}
			}

			if (count == 0)
			{
				string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
				throw new ApplicationException(
					$"Can't find any type which implements ISource in {assembly} from {assembly.Location}.\n" +
					$"Available types: {availableTypes}");
			}
		}
	}
}
