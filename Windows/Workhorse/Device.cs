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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using HardwareMonitor.Protocol;

namespace HardwareMonitor
{
	public enum eMicrocontroller
	{
		Teensy32,
		Teensy40,
		SeeediunoXiao,

		Unknown
	}

	public enum eScreen
	{
		ILI9341,
		ILI9486,
		ILI9488,
		NT35510,

		Unknown
	}

	public enum eResolution
	{
		[XmlEnum("240x320")]
		R240x320,

		[XmlEnum("320x480")]
		R320x480,

		[XmlEnum("480x800")]
		R480x800,

		Unknown
	}

	public enum Orientation
	{
		Vertical,
		Horizontal
	}

	public class Device
	{
		#region Private Fields

		private bool _onceOnlyDataSent = false;
		private Connection.ActiveConnection _activeConnection;

		private Layout.Config _layout;

		private eMicrocontroller _microcontroller = eMicrocontroller.Unknown;
		private eScreen _screen = eScreen.Unknown;
		private eResolution _resolution = eResolution.Unknown;

		private int _versionMajor;
		private int _versionMinor;
		private int _versionPatch;

		#endregion Private Fields

		#region Public Properties

		public Connection.ActiveConnection Connection
		{
			get
			{
				return _activeConnection;
			}

			set
			{
				_activeConnection = value;
				_activeConnection.DataRecieved += _activeConnection_DataRecieved;
				RequestIdentity();
				RequestVersion();
			}
		}

		public Icon.Config Icons { get; set; }
		public bool IsConnected => Connection is not null && Connection.IsOpen;

		public Layout.Config Layout => _layout;

		public eMicrocontroller Microcontroller => _microcontroller;
		public eScreen Screen => _screen;
		public eResolution Resolution => _resolution;
		public Orientation Orientation => Orientation.Vertical;

		public string Version => $"{_versionMajor}.{_versionMinor}.{_versionPatch}";

		#endregion Public Properties

		#region Events

		public delegate void SimpleDeviceEventHandler(Device device);
		public event SimpleDeviceEventHandler IdentityRecieved;

		#endregion Events

		#region Public Methods

		public async Task Update(Monitor.Snapshot snapshot)
		{
			if (Connection == null)
				return;

			if (!Connection.IsOpen)
				return;

			foreach (var mappedComponent in _layout.MappedComponents)
			{
				await UpdateMappedComponent(snapshot, mappedComponent.Key, mappedComponent.Value);
			}

			_onceOnlyDataSent = true;
		}

		public async Task SetLayout(Layout.Config layout)
		{
			if (!IsConnected)
				return;

			//@todo: Validate layout against device, make sure it's suitable

			_layout = layout;

			Connection.GuaranteedPacket packet = new Connection.GuaranteedPacket();
			packet.Connection =  Connection;

			dynamic[] args = layout.CollectValues();
			await packet.SendAsync(args);
		}

		#endregion Public Methods

		#region Private Methods

		private async Task UpdateMappedComponent(Monitor.Snapshot snapshot, string useCapture, List<Layout.MappedComponent> mappedComponents)
		{
			Monitor.Capture capture;
			if (snapshot.Captures.TryGetValue(useCapture, out capture))
			{
				foreach(Layout.MappedComponent mappedComponent in mappedComponents)
				{
					if (mappedComponent.Component.NoUpdate && _onceOnlyDataSent)
						continue;

					if (mappedComponent.Component.NoUpdate)
					{
						if (mappedComponent.Component is Layout.Icon && Icons != null)
						{
							string iconName = Icons.GetIcon(capture.Value);

							if (iconName != null)
							{
								string iconPath = Path.Join("Images", $"{iconName}.bmp");
								HardwareMonitor.Connection.IconSender.Send(mappedComponent, iconPath, Connection);
							}
						}
						else
						{
							Connection.GuaranteedPacket packet = new Connection.GuaranteedPacket();
							packet.Connection = Connection;
							await packet.SendAsync(HardwareMonitor.Protocol.PacketType.ModuleUpdate, mappedComponent.ModuleIndex, (byte)1, mappedComponent.ComponentIndex, capture.Value);
						}
					}
					else
					{
						Connection.SimplePacket packet = new Connection.SimplePacket();
						packet.Connection = Connection;
						packet.Send(HardwareMonitor.Protocol.PacketType.ModuleUpdate, mappedComponent.ModuleIndex, (byte)1, mappedComponent.ComponentIndex, capture.Value);
					}
				}
			}
		}

		private void RequestVersion()
		{
			Connection.SimplePacket packet = new Connection.SimplePacket();
			packet.Connection = Connection;
			packet.Send(HardwareMonitor.Protocol.PacketType.VersionRequest);
		}

		private void RequestIdentity()
		{
			Connection.SimplePacket packet = new Connection.SimplePacket();
			packet.Connection = Connection;
			packet.Send(HardwareMonitor.Protocol.PacketType.IdentityRequest);
		}

		#endregion Private Methods

		#region Event Handlers

		private void _activeConnection_DataRecieved(Connection.ActiveConnection connection, byte[] data, int dataLength)
		{
			PacketType packetType = (PacketType)data[0];
			switch (packetType)
			{
				case PacketType.Identity:
					_microcontroller = (eMicrocontroller)data[1];
					_screen = (eScreen)data[2];
					_resolution = (eResolution)data[3];

					IdentityRecieved?.Invoke(this);
					break;

				case PacketType.Version:
					_versionMajor = data[1];
					_versionMinor = data[2];
					_versionPatch = data[3];
					break;
			}
		}

		#endregion Event Handlers
	}
}
