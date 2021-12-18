using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

namespace HardwareMonitor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TaskbarIcon _tb;

		private async void Application_Startup(object sender, StartupEventArgs e)
		{
			//initialize NotifyIcon
			_tb = (TaskbarIcon)FindResource("MyNotifyIcon");

			// Instanticate the class that will perform the main body of work for us
			Main program = new Main();

			// Run the application
			int retcode = await program.Run();
		}
	}
}
