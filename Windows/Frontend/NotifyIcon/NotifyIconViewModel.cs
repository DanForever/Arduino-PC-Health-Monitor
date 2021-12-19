using System;
using System.Windows;
using System.Windows.Input;

namespace HardwareMonitor.NotifyIcon
{
	class NotifyIconViewModel
	{
		private ICommand _exitApplicationCommand = new DelegateCommand { CommandAction = () => { ((App)Application.Current).Program.RequestExit = true; } };

		public ICommand ExitApplicationCommand => _exitApplicationCommand;
	}


	public class DelegateCommand : ICommand
	{
		public Action CommandAction { get; set; }
		public Func<bool> CanExecuteFunc { get; set; }

		public void Execute(object parameter)
		{
			CommandAction();
		}

		public bool CanExecute(object parameter)
		{
			return CanExecuteFunc == null || CanExecuteFunc();
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
