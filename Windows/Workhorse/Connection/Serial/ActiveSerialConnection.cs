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
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.Versioning;
using System.Text;

[assembly:UnsupportedOSPlatform("ios")]
[assembly:UnsupportedOSPlatform("android")]
namespace HardwareMonitor.Connection.Serial
{
	internal class ActiveConnection : Connection.ActiveConnection
	{
		#region Private Fields

		private string _name;
		private SerialPort _serialPort = new SerialPort();
		private DataReciever _dataReciever = new DataReciever();
		private event Connection.ActiveConnection.DataRecievedHandler _dataRecievedEvent;

		#endregion Private Fields

		#region Connection.ActiveConnection

		string Connection.ActiveConnection.Name => _name;

		bool Connection.ActiveConnection.IsOpen => _serialPort.IsOpen;

		event Connection.ActiveConnection.DataRecievedHandler Connection.ActiveConnection.DataRecieved { add { _dataRecievedEvent += value; } remove { _dataRecievedEvent -= value; } }

		void Connection.ActiveConnection.Send(byte[] data)
		{
			try
			{
				_serialPort.Write(data, 0, data.Length);
			}
			catch(InvalidOperationException)
			{
				throw new ConnectionClosedException(this);
			}
		}

		void Connection.ActiveConnection.Disconnect()
		{
			_serialPort.Close();
		}

		#endregion Connection.ActiveConnection

		#region C-Tor

		public ActiveConnection(AvailableConnection availableSerialConnection)
		{
			_name = availableSerialConnection.Name;

			_serialPort.PortName = availableSerialConnection.Name;
			_serialPort.DataReceived += _serialPort_DataReceived;

			try
			{
				_serialPort.Open();
				_serialPort.DtrEnable = true;
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new ConnectionFailedException("Invalid serial port") { AvailableConnection = availableSerialConnection };
			}
			catch (UnauthorizedAccessException)
			{
				throw new ConnectionFailedException($"Serial port {availableSerialConnection.Name} is already open by another application") { AvailableConnection = availableSerialConnection };
			}

			_dataReciever.DebugRecieved += _dataReciever_DebugRecieved;
			_dataReciever.DataRecieved += _dataReciever_DataRecieved;
		}

		#endregion C-Tor

		#region Event Handlers

		private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			int bytesToRead = _serialPort.BytesToRead;
			byte[] data = new byte[bytesToRead];
			_serialPort.Read(data, 0, data.Length);

			_dataReciever.Recieve(data);
		}

		private void _dataReciever_DebugRecieved(byte[] data, int length, bool matchedNewline)
		{
			string debugString = Encoding.UTF8.GetString(data);

			Debug.Write(debugString);
		}

		private void _dataReciever_DataRecieved(byte[] data, int length, bool matchedNewline)
		{
			_dataRecievedEvent?.Invoke(this, data, length);
		}

		#endregion Event Handlers
	}
}
