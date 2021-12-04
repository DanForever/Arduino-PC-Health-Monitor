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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace HardwareMonitor.Connection
{
    // @todo: Move out of Serial

    class DataRecieverInternal
    {
        private byte[] _pattern;

        private byte[] _intermediateBuffer = new byte[32];
        private int _intermediateLength = 0;

        private byte[] _data = new byte[2048];
        private int _dataLength = 0;

        public bool NewlineEndsPacket { get; set; } = false;

        public delegate void DataRecievedHandler(byte[] data, int length, bool matchedNewline);
        public event DataRecievedHandler DataRecieved;

        public DataRecieverInternal(byte[] pattern)
        {
            _pattern = pattern;
        }

        public int Recieve(ArraySegment<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                _intermediateBuffer[_intermediateLength] = data[i];
                ++_intermediateLength;

                if (!CouldBePattern(_intermediateBuffer, _intermediateLength, _pattern))
                {
                    FlushIntermediateBuffer();
                }
                else if (_intermediateLength == _pattern.Length)
                {
                    // Discard the footer, we don't need it
                    _intermediateLength = 0;

                    DataRecieved?.Invoke(_data, _dataLength, false);

                    ClearDataBuffer();

                    return i + 1;
                }
                
                if(NewlineEndsPacket && _dataLength > 0 && _data[_dataLength - 1] == (byte)'\n')
                {
                    DataRecieved?.Invoke(_data, _dataLength, true);

                    ClearDataBuffer();

                    return i + 1;
                }
            }

            return data.Count;
        }

        private static bool CouldBePattern(byte[] data, int dataLength, byte[] pattern)
        {
            int maxLength = Math.Min(dataLength, pattern.Length);

            for (int i = 0; i < maxLength; ++i)
            {
                if (data[i] != pattern[i])
                    return false;
            }

            return true;
        }

        private void FlushIntermediateBuffer()
        {
            Array.Copy(_intermediateBuffer, 0, _data, _dataLength, _intermediateLength);
            _dataLength += _intermediateLength;
            _intermediateLength = 0;
        }

        private void ClearDataBuffer()
        {
            for (int i = 0; i < _dataLength; ++i)
            {
                _data[i] = 0;
            }

            _dataLength = 0;
        }
    }


    class DataReciever
    {
        public static byte[] Header => Encoding.ASCII.GetBytes("dan<`");
        public static byte[] Footer => Encoding.ASCII.GetBytes("`>dan");

        DataRecieverInternal _processUntilHeader = new DataRecieverInternal(Header) { NewlineEndsPacket = true };
        DataRecieverInternal _processUntilFooter = new DataRecieverInternal(Footer);
        DataRecieverInternal _activeProcessor;

        public event DataRecieverInternal.DataRecievedHandler DataRecieved { add { _processUntilFooter.DataRecieved += value; } remove { _processUntilFooter.DataRecieved -= value; } }
        public event DataRecieverInternal.DataRecievedHandler DebugRecieved { add { _processUntilHeader.DataRecieved += value; } remove { _processUntilHeader.DataRecieved -= value; } }

        public DataReciever()
        {
            _activeProcessor = _processUntilHeader;
            _processUntilHeader.DataRecieved += ProcessingComplete;
            _processUntilFooter.DataRecieved += ProcessingComplete;
        }

        private void ProcessingComplete(byte[] data, int length, bool matchedNewline)
        {
            if (matchedNewline)
                return;

            if (_activeProcessor == _processUntilHeader)
                _activeProcessor = _processUntilFooter;
            else
                _activeProcessor = _processUntilHeader;
        }

        public void Recieve(byte[] data)
        {
            int offset = 0;

            do
            {
                int remaining = data.Length - offset;
                ArraySegment<byte> segment = new ArraySegment<byte>(data, offset, remaining);

                // The active processor will be switched out by the event that fires
                offset += _activeProcessor.Recieve(segment);
            } while (offset < data.Length);
        }
    }

    class ActiveSerialConnection : ActiveConnection
    {
		private string _name;
        private SerialPort _serialPort = new SerialPort();
        private DataReciever _dataReciever = new DataReciever();
        private event ActiveConnection.DataRecievedHandler _dataRecievedEvent;

		string ActiveConnection.Name => _name;

		bool ActiveConnection.IsOpen => _serialPort.IsOpen;

		public ActiveSerialConnection(AvailableSerialConnection availableSerialConnection)
        {
			_name = availableSerialConnection.Name;

			_serialPort.PortName = availableSerialConnection.Name;
            _serialPort.DataReceived += _serialPort_DataReceived;

			try
			{
				_serialPort.Open();
			}
			catch(System.IO.FileNotFoundException)
			{
				throw new ConnectionFailedException("Invalid serial port") { AvailableConnection = availableSerialConnection };
			}
			catch(UnauthorizedAccessException)
			{
				throw new ConnectionFailedException($"Serial port {availableSerialConnection.Name} is already open by another application") { AvailableConnection = availableSerialConnection };
			}

            _dataReciever.DebugRecieved += _dataReciever_DebugRecieved;
            _dataReciever.DataRecieved += _dataReciever_DataRecieved;
        }

        event ActiveConnection.DataRecievedHandler ActiveConnection.DataRecieved { add { _dataRecievedEvent += value; } remove { _dataRecievedEvent -= value; } }

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

        void ActiveConnection.Send(byte[] data)
        {
            _serialPort.Write(data, 0, data.Length);
        }
    }

    class AvailableSerialConnection : AvailableConnection
    {
        private string _name;

        public override string Name => _name;

        public AvailableSerialConnection(string name) => _name = name;

		public override ActiveConnection Connect()
		{
			return new ActiveSerialConnection(this);
		}
	}

    class Serial
    {
        public static IEnumerable<AvailableConnection> EnumeratePorts()
        {
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                yield return new AvailableSerialConnection(port);
            }
        }
    }
}
