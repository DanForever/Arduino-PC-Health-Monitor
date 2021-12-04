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
using System.Threading.Tasks;

namespace HardwareMonitor.Testing
{
	class Program
	{
		static async Task<int> Main(string[] args)
		{
			string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			Console.WriteLine("Dan's hardware monitor");

			// Instanticate the class that will perform the main body of work for us
			Main program = new Main();

			// Hook up the feedback so it's printed to the console
			program.Feedback += (string text) => Console.WriteLine(text);

			// Listen out for an exit request
			Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
			{
				Console.WriteLine("Exit requested...");

				// Cancel the OS based force-quit and instead allow the application to quit gracefully
				e.Cancel = true;
				program.RequestExit = true;
			};

			// Run the application
			int retcode = await program.Run();

			// Some final user feedback
			Console.WriteLine("Exit complete");

			return retcode;
		}
	}
}
