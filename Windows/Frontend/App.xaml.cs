using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

namespace HardwareMonitor
{
	public partial class App : Application
	{
		public static App Application => (App)Current;

		private TaskbarIcon _tb;

		public Main Program { get; set; }

		private async void Application_Startup(object sender, StartupEventArgs e)
		{
			// Instanticate the class that will perform the main body of work for us
			Program = new Main();

			//initialize NotifyIcon
			_tb = (TaskbarIcon)FindResource("MyNotifyIcon");

			// Run the application
			int retcode = await Program.Run();

			Shutdown(retcode);
		}
	}
}
