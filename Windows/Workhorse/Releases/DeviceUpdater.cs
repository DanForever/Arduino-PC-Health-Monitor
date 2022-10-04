/*
 *   Arduino PC Health Monitor (PC Companion app)
 *   Polls the hardware sensors for data and forwards them on to the arduino device
 *   Copyright (C) 2022 Daniel Neve
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

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HardwareMonitor.Releases
{
	public static class DeviceUpdater
	{
		#region Public properties

		public static bool Initialised { get; set; } = false;

		public static string ArduinoCLIWorkspacePath => Path.Join(Environment.LocalAppdata, "arduino-cli");
		public static string ArduinoCLIConfigPath => Path.Join(ArduinoCLIWorkspacePath, "config.yml");
		public static string ArduinoCLIExecutableDevPath => Path.Join(Environment.ExecutableFolder, "..", "..", "External", "arduino-cli.exe");
		public static string ArduinoCLIExecutableInstalledPath => Path.Join(Environment.ExecutableFolder, "Tools", "arduino-cli.exe");
		public static string ArduinoCLIExecutablePath
		{
			get
			{
				if (File.Exists(ArduinoCLIExecutableInstalledPath)) return ArduinoCLIExecutableInstalledPath;
				if (File.Exists(ArduinoCLIExecutableDevPath)) return ArduinoCLIExecutableDevPath;
				return null;
			}
		}

		public static bool ArduinoCLIConfigExists => File.Exists(ArduinoCLIConfigPath);
		public static string ExtractedBinariesWorkspacePath => Path.Join(Environment.LocalAppdata, "downloaded-binaries");
		public static string ExtractedBinaryPath => Path.Join(Environment.LocalAppdata, "downloaded-binaries", "device.bin");

		#endregion Public properties

		#region Public Methods

		public static async Task Initialise()
		{
			if(!ArduinoCLIConfigExists)
			{
				CreateArduinoCLIConfig();
				await DoArduinoCLIFirstTimeSetup();
			}

			Initialised = true;
		}

		public static async Task Update(Device device, string pathToZip)
		{
			if (!Initialised)
				return;

			PrepareBinaryWorkspace();
			UnpackBinary(pathToZip);

			string pathToBinary = ExtractedBinaryPath;

			switch (device.Microcontroller)
			{
			case eMicrocontroller.Xiao2040:
				if (Path.GetExtension(pathToBinary) != ".uf2")
				{
					string newPathToBinary = Path.ChangeExtension(pathToBinary, ".uf2");
					File.Move(pathToBinary, newPathToBinary);
					pathToBinary = newPathToBinary;
				}
				
				break;
			}

			// fully qualified board name
			string fqbn = "";

			// @todo: Move this to a config file
			switch (device.Microcontroller)
			{
			case eMicrocontroller.Xiao2040:
				fqbn = "rp2040:rp2040:seeed_xiao_rp2040";
				break;
			}

			device.DoNotRemove = true;
			await device.Lock.WaitAsync();
			device.Connection.Disconnect();

			await RunProcess(ArduinoCLIExecutablePath, $"--config-file \"{ArduinoCLIConfigPath}\" upload -t -v -p {device.Connection.Name} -i \"{pathToBinary}\" -b {fqbn}");

			device.Lock.Release();
			device.DoNotRemove = false;
		}

		#endregion Public Methods

		#region Private Methods

		private static void CreateArduinoCLIConfig()
		{
			Directory.CreateDirectory(ArduinoCLIWorkspacePath);
			using (StreamWriter stream = new StreamWriter(ArduinoCLIConfigPath))
			{
				stream.WriteLine("directories:");
				stream.WriteLine($"  data: {Path.Join(ArduinoCLIWorkspacePath, "data")}");
				stream.WriteLine($"  downloads: {Path.Join(ArduinoCLIWorkspacePath, "downloads")}");
				stream.WriteLine($"  user: {Path.Join(ArduinoCLIWorkspacePath, "user")}");
				stream.WriteLine("board_manager:");
				stream.WriteLine("  additional_urls:");
				stream.WriteLine("    - https://github.com/earlephilhower/arduino-pico/releases/download/global/package_rp2040_index.json");
				stream.WriteLine("    - https://www.pjrc.com/teensy/package_teensy_index.json");
			}
		}

		private static async Task DoArduinoCLIFirstTimeSetup()
		{
			await RunProcess(ArduinoCLIExecutablePath, $"--config-file \"{ArduinoCLIConfigPath}\" core update-index");
			await RunProcess(ArduinoCLIExecutablePath, $"--config-file \"{ArduinoCLIConfigPath}\" core install rp2040:rp2040@2.5.4");
			await RunProcess(ArduinoCLIExecutablePath, $"--config-file \"{ArduinoCLIConfigPath}\" core install teensy:avr@1.57.0");
		}

		private static async Task RunProcess(string exe, string arguments)
		{
			Process process = new();

			process.StartInfo.FileName = exe;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
			{
				Debug.WriteLine($"DeviceUpdater: {e.Data}");
			});

			process.Start();
			process.BeginOutputReadLine();

			await process.WaitForExitAsync();
		}

		private static void PrepareBinaryWorkspace()
		{
			if(Directory.Exists(ExtractedBinariesWorkspacePath))
				Directory.Delete(ExtractedBinariesWorkspacePath, true);
			Directory.CreateDirectory(ExtractedBinariesWorkspacePath);
		}

		private static void UnpackBinary(string pathToZip)
		{
			// Unzip the binary
			System.IO.Compression.ZipFile.ExtractToDirectory(pathToZip, ExtractedBinariesWorkspacePath);
		}

		#endregion Private Methods
	}
}
