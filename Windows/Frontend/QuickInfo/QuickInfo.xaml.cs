using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HardwareMonitor.QuickInfo
{
	public class DeviceInfo : INotifyPropertyChanged
	{
		private Device _device;

		public string Port => _device.Connection.Name;
		public string Layout => _device.Layout?.Name ?? "No Layout set";
		public Orientation Orientation => _device.Orientation;

		public DeviceInfo(Device device)
		{
			_device = device;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public partial class QuickInfo : UserControl
	{
		public ObservableCollection<DeviceInfo> Devices
		{
			get => (ObservableCollection<DeviceInfo>)GetValue(DevicesProperty);
			private set => SetValue(DevicesProperty, value);
		}

		public static readonly DependencyProperty DevicesProperty = DependencyProperty.Register(nameof(Devices), typeof(ObservableCollection<DeviceInfo>), typeof(QuickInfo));

		public QuickInfo()
		{
			InitializeComponent();
			PopulateDeviceInfo();

			App.Application.Program.DeviceConnected += Program_DeviceConnected;
			App.Application.Program.DeviceDisconnected += Program_DeviceDisconnected;

			Devices = new ObservableCollection<DeviceInfo>();
		}

		private void Program_DeviceConnected(Device device)
		{
			Devices.Add(new DeviceInfo(device));
		}

		private void Program_DeviceDisconnected(Device device)
		{
			DeviceInfo deviceInfo = Devices.First(d => d.Port == device.Connection.Name);
			Devices.Remove(deviceInfo);
		}

		private void PopulateDeviceInfo()
		{
			foreach(Device device in App.Application.Program.Devices)
			{
				Devices.Add(new DeviceInfo(device));
			}
		}
	}
}
