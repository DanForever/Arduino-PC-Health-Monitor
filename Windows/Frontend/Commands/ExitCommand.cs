using System.Windows.Input;

namespace HardwareMonitor
{
	static partial class CustomCommands
	{
		public static readonly RoutedUICommand Exit = new
		(
			"Exit",
			"Exit",
			typeof(CustomCommands),
			new InputGestureCollection()
			{
				new KeyGesture(Key.F4, ModifierKeys.Alt)
			}
		);
	}
}
