using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HardwareMonitor.Releases
{
	public static class Releases
	{
		#region Internal Types

		internal enum eType
		{
			CompanionApp,
			Device
		}

		internal class Release
		{
			#region Public Properties (Populated from json data)

			[JsonPropertyName("name")]
			public string Name { get; set; }

			[JsonPropertyName("tag_name")]
			public string Tag { get; set; }

			[JsonPropertyName("url")]
			public string Url { get; set; }

			[JsonPropertyName("html_url")]
			public string HtmlUrl { get; set; }

			[JsonPropertyName("assets")]
			public Asset[] Assets { get; set; }

			[JsonPropertyName("published_at")]
			public DateTime ReleasedOn { get; set; }

			#endregion Public Properties (Populated from json data)

			#region Public Properties (Parsed from other properties)

			// Implicit properties
			[JsonIgnore]
			public Version Version { get; set; }

			[JsonIgnore]
			public eType Type { get; set; }

			#endregion Public Properties (Parsed from other properties)
		}

		internal class Asset
		{
			#region Public Properties (Populated from json data)

			[JsonPropertyName("name")]
			public string Name { get; set; }

			[JsonPropertyName("url")]
			public string Url { get; set; }

			[JsonPropertyName("browser_download_url")]
			public string DownloadUrl { get; set; }

			#endregion Public Properties (Populated from json data)

			#region Public Properties (Parsed from other properties)

			/// <summary>
			/// Only applicable for devices
			/// </summary>
			[JsonIgnore]
			public eMicrocontroller Microcontroller { get; set; }

			/// <summary>
			/// Only applicable for devices
			/// </summary>
			[JsonIgnore]
			public eScreen Screen { get; set; }

			#endregion Public Properties (Parsed from other properties)
		}

		/// <summary>
		/// This class exists to provide a nice bit of syntactic sugar:
		/// Latest.ReleaseType
		/// </summary>
		internal static class Latest
		{
			internal static Release CompanionApp { get; set; }
			internal static Release Devices { get; set; }
		}

		internal abstract class DeviceUpdater
		{
			public abstract bool Ready { get; }

			public abstract void Update(string pathToDownloadedAsset);
		}

		internal class TeensyUpdater : DeviceUpdater
		{
			public eMicrocontroller Microcontroller { get; set; } = eMicrocontroller.Unknown;

			public override bool Ready => FindPathToTeensyLoader() != null && GetMCUCode() != null;

			public override void Update(string pathToDownloadedAsset)
			{
				string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				Directory.CreateDirectory(tempDirectory);

				System.IO.Compression.ZipFile.ExtractToDirectory(pathToDownloadedAsset, tempDirectory);

				string teensyLoaderPath = FindPathToTeensyLoader();
				string hexBinaryPath = FindPathToHexBinary(tempDirectory);

				if(hexBinaryPath == null)
				{
					Debug.WriteLine($"Couldn't find the correct binary file in '{tempDirectory}'");
					return;
				}

				Process teensyLoaderProcess = new();

				teensyLoaderProcess.StartInfo = new ProcessStartInfo(teensyLoaderPath)
				{
					Arguments = $"--mcu={GetMCUCode()} -w {hexBinaryPath}" 
				};

				teensyLoaderProcess.Start();
			}

			private string FindPathToTeensyLoader()
			{
				string installPath = "Tools/teensy_loader_cli.exe";
				if (File.Exists(installPath))
					return installPath;

				string devPath = "../../External/teensy_loader_cli.exe";
				if (File.Exists(devPath))
					return devPath;

				return null;
			}

			/// <summary>
			/// There should only be a single file in the directory in which the downloaded file was unzipped
			/// </summary>
			private string FindPathToHexBinary(string directory)
			{
				string[] files = Directory.GetFiles(directory);

				if (files.Length != 1)
					return null;

				return files[0];
			}

			private string GetMCUCode()
			{
				switch (Microcontroller)
				{
				case eMicrocontroller.Teensy32:
					return "TEENSY32";

				case eMicrocontroller.Teensy40:
					return "TEENSY40";

				default:
					return null;
				}
			}
		}

		#endregion Internal Types

		#region Public Properties

		/// <summary>
		/// Returns true if there is a newer version available for download
		/// </summary>
		public static bool CompanionAppUpdateAvailable => Latest.CompanionApp?.Version > Assembly.GetExecutingAssembly().GetName().Version;

		public static Version LatestDeviceVersionAvailable => Latest.Devices?.Version;

		#endregion Public Properties

		#region Public Methods

		/// <summary>
		/// This will check the releases on github and see if the currently executing binary is the latest release
		/// </summary>
		/// <returns></returns>
		public static async Task UpdateLatestReleases()
		{
			// Create a new instance of WebClient, this will be the tool we use to interact with website REST APIs
			using (var client = new WebClient())
			{
				// The github REST api will give us a 403 forbidden error if we do not supply a user agent
				client.Headers.Add("user-agent", $"hardware-monitor-app/{Assembly.GetExecutingAssembly().GetName().Version}");

				// The github REST api documentation recommends we specify an "Accept" header so we can be sure of the format of the data we're recieving
				client.Headers.Add("Accept", $"application/vnd.github.v3+json");

				// This is the URL to query the repository for releases
				// "per_page=4" limits the number of releases to the most recent 4 (normally it returns up to 30). Technically we only need the latest 2 (app + devices) but 4 gives us a little additional safety
				const string uri = "https://api.github.com/repos/DanForever/Arduino-PC-Health-Monitor/releases?per_page=4";

				// Initiate the query!
				string json = await client.DownloadStringTaskAsync(uri);

				// Process the response!
				var releases = JsonSerializer.Deserialize<List<Release>>(json);

				// Prepare the regex that will parse the version number and type of release from the release names
				string pattern = @"([\w-]+)\.v(\d+)\.(\d+)\.(\d+)";
				Regex rgx = new(pattern);

				foreach (Release release in releases)
				{
					// Execute the regex
					var match = rgx.Match(release.Name);

					// If the regex didn't match, then we can't get the information we need from the release, so we must disregard it
					if (!match.Success)
					{
						Debug.WriteLine($"Release with name \"{release.Name}\" does not fit the naming convention, so it's ignored");
						continue;
					}

					// This assert should only fail if the regex is incorrect
					Debug.Assert(match.Groups.Count == 5, "There should be 5 matches", "The whole string, the name, the 3 parts of the version");

					// Release-Name.vMajor.Minor.Patch
					// 111111111111  22222 33333 44444
					// 0000000000000000000000000000000
					release.Version = new(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

					if (match.Groups[1].Value == "companion-app")
					{
						release.Type = eType.CompanionApp;
					}
					else if (match.Groups[1].Value == "devices")
					{
						release.Type = eType.Device;
					}

					switch (release.Type)
					{
					case eType.CompanionApp:
						if (Latest.CompanionApp == null || Latest.CompanionApp.Version < release.Version)
						{
							Latest.CompanionApp = release;
						}
						break;

					case eType.Device:
						if (Latest.Devices == null || Latest.Devices.Version < release.Version)
						{
							Latest.Devices = release;
						}
						break;
					}

					ParseAdditionalAssetProperties(release);
				}
			}
		}

		/// <summary>
		/// Download the installer, run it
		/// </summary>
		public static async Task DownloadAndRunLatestCompanionAppInstaller()
		{
			if (Latest.CompanionApp == null)
				return;

			// x86 or x64?
			Asset asset = FindCorrectCompanionAppAsset(Latest.CompanionApp);
			string downloadedPath = GenerateDownloadPath(asset.Name);

			using (var client = new WebClient())
			{
				// The github REST api will give us a 403 forbidden error if we do not supply a user agent
				client.Headers.Add("user-agent", $"hardware-monitor-app/{Assembly.GetExecutingAssembly().GetName().Version}");

				// Download the file!
				await client.DownloadFileTaskAsync(asset.DownloadUrl, downloadedPath);
			}

			// Run the installer!
			Process installer = new();

			installer.StartInfo = new ProcessStartInfo(downloadedPath)
			{
				UseShellExecute = true
			};

			installer.Start();
		}

		public static async Task UpdateDeviceFirmware(Device device)
		{
			if (!(device.Version < LatestDeviceVersionAvailable))
			{
				Debug.WriteLine($"Device '{device.Connection.Name}' already has the latest firmware");
				return;
			}

			Asset asset = FindCorrectDeviceAsset(device, Latest.Devices);

			if(asset == null)
			{
				Debug.WriteLine($"Could not find a suitable release for Device '{device.Connection.Name}' with {device.Microcontroller} and {device.Screen}");
				return;
			}

			DeviceUpdater deviceUpdater = CreateDeviceUpdater(device);

			if(deviceUpdater == null || !deviceUpdater.Ready)
			{
				Debug.WriteLine($"Device of type '{device.Microcontroller}' does not have automatic update support yet");
				return;
			}

			string downloadedPath = GenerateDownloadPath(asset.Name);

			using (var client = new WebClient())
			{
				// The github REST api will give us a 403 forbidden error if we do not supply a user agent
				client.Headers.Add("user-agent", $"hardware-monitor-app/{Assembly.GetExecutingAssembly().GetName().Version}");

				// Download the file!
				await client.DownloadFileTaskAsync(asset.DownloadUrl, downloadedPath);
			}

			deviceUpdater.Update(downloadedPath);
		}

		#endregion Public Methods

		#region Private Methods

		private static DeviceUpdater CreateDeviceUpdater(Device device)
		{

			switch (device.Microcontroller)
			{
			case eMicrocontroller.Teensy32:
			case eMicrocontroller.Teensy40:
				return new TeensyUpdater() { Microcontroller = device.Microcontroller };

			default:
				return null;
			}
		}

		private static string GenerateDownloadPath(string sourceFilename, int suffix)
		{
			string body = Path.GetFileNameWithoutExtension(sourceFilename);
			string ext = Path.GetExtension(sourceFilename);

			string now = DateTime.Now.ToString("yyyyMMddHHmmss");

			return Path.Combine(Path.GetTempPath(), $"{body}.{now}.{suffix}{ext}");
		}

		private static string GenerateDownloadPath(string sourceFilename)
		{
			const int MaxAttempts = 10;

			for (int i = 0; i < MaxAttempts; ++i)
			{
				string potentialPath = GenerateDownloadPath(sourceFilename, i);

				if (!File.Exists(potentialPath))
					return potentialPath;
			}

			return null;
		}

		private static Asset FindCorrectCompanionAppAsset(Release release)
		{
			string platformString = Environment.Is64BitProcess ? "x64" : "x86";

			foreach (Asset asset in release.Assets)
			{
				if (asset.Name.Contains(platformString))
					return asset;
			}

			return null;
		}

		private static Asset FindCorrectDeviceAsset(Device device, Release release)
		{
			foreach (Asset asset in release.Assets)
			{
				if (asset.Microcontroller != device.Microcontroller)
					continue;

				if (asset.Screen != device.Screen)
					continue;

				return asset;
			}

			return null;
		}

		private static void ParseAdditionalAssetProperties(Release release)
		{
			switch(release.Type)
			{
			case eType.Device:
				foreach (Asset asset in release.Assets)
				{
					ParseAdditionalDeviceAssetProperties(asset);
				}
				break;
			}
		}

		private static void ParseAdditionalDeviceAssetProperties(Asset asset)
		{
			char[] separators = new char[] { '-', '.' };
			string[] parts = asset.Name.Split(separators);

			if (parts.Length < 2)
			{
				Debug.WriteLine($"Asset '{asset.Name}' has incorrectly formatted name and cannot be processed");
				return;
			}

			eMicrocontroller microcontroller = eMicrocontroller.Unknown;
			Enum.TryParse(parts[0], true, out microcontroller);

			eScreen screen = eScreen.Unknown;
			Enum.TryParse(parts[1], true, out screen);

			Debug.WriteLine($"Found asset with microcontroller {microcontroller} and Screen {screen}");

			asset.Microcontroller = microcontroller;
			asset.Screen = screen;
		}

		#endregion Private Methods
	}
}
