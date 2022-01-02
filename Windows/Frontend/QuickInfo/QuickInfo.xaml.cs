using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HardwareMonitor.QuickInfo
{
	public class DeviceInfo : INotifyPropertyChanged
	{
		#region Private Fields

		private Device _device;

		#endregion Private Fields

		#region Public Properties

		public string Port => _device.Connection.Name;
		public string Layout => _device.Layout?.Name ?? "No Layout set";
		public Orientation Orientation => _device.Orientation;

		#endregion Public Properties

		#region C-Tor

		public DeviceInfo(Device device)
		{
			_device = device;
		}

		#endregion C-Tor

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion INotifyPropertyChanged
	}

	public partial class QuickInfo : UserControl
	{
		#region Public Properties
		
		public ObservableCollection<DeviceInfo> Devices
		{
			get => (ObservableCollection<DeviceInfo>)GetValue(DevicesProperty);
			private set => SetValue(DevicesProperty, value);
		}

		#endregion Public Properties

		#region Dependency Properties

		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register(nameof(Devices), typeof(ObservableCollection<DeviceInfo>), typeof(QuickInfo));

		#endregion Dependency Properties

		#region C-Tor

		public QuickInfo()
		{
			InitializeComponent();
			PopulateDeviceInfo();

			App.Application.Program.DeviceConnected += Program_DeviceConnected;
			App.Application.Program.DeviceDisconnected += Program_DeviceDisconnected;

			Devices = new ObservableCollection<DeviceInfo>();
		}

		#endregion C-Tor

		#region Event Handlers

		private void Program_DeviceConnected(Device device)
		{
			Devices.Add(new DeviceInfo(device));
		}

		private void Program_DeviceDisconnected(Device device)
		{
			DeviceInfo deviceInfo = Devices.First(d => d.Port == device.Connection.Name);
			Devices.Remove(deviceInfo);
		}

		#endregion Event Handlers

		#region Private Methods

		private void PopulateDeviceInfo()
		{
			foreach(Device device in App.Application.Program.Devices)
			{
				Devices.Add(new DeviceInfo(device));
			}
		}

		#endregion Private Methods
	}
}
