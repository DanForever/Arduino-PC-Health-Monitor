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
		public string Version => _device.Version;

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
