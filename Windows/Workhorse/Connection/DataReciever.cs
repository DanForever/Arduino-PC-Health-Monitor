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
using System.Text;

namespace HardwareMonitor.Connection
{
	internal class DataRecieverInternal
    {
		#region Private Fields

		private byte[] _pattern;

        private byte[] _intermediateBuffer = new byte[32];
        private int _intermediateLength = 0;

        private byte[] _data = new byte[2048];
        private int _dataLength = 0;

		#endregion Private Fields

		#region Public Properties

		public bool NewlineEndsPacket { get; set; } = false;

		#endregion Public Properties

		#region Events

		public delegate void DataRecievedHandler(byte[] data, int length, bool matchedNewline);
        public event DataRecievedHandler DataRecieved;

		#endregion Events

		#region C-Tor

		public DataRecieverInternal(byte[] pattern)
        {
            _pattern = pattern;
        }

		#endregion C-Tor

		#region Public Methods

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

		#endregion Public Methods

		#region Private Methods

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

		#endregion Private Methods
	}

	internal class DataReciever
    {
		#region Public consts

		// @todo: Consolidate
		public static byte[] Header => Encoding.ASCII.GetBytes("dan<`");
        public static byte[] Footer => Encoding.ASCII.GetBytes("`>dan");

		#endregion Public consts

		#region Private Fields

		DataRecieverInternal _processUntilHeader = new DataRecieverInternal(Header) { NewlineEndsPacket = true };
        DataRecieverInternal _processUntilFooter = new DataRecieverInternal(Footer);
        DataRecieverInternal _activeProcessor;

		#endregion Private Fields

		#region Events

		public event DataRecieverInternal.DataRecievedHandler DataRecieved { add { _processUntilFooter.DataRecieved += value; } remove { _processUntilFooter.DataRecieved -= value; } }
        public event DataRecieverInternal.DataRecievedHandler DebugRecieved { add { _processUntilHeader.DataRecieved += value; } remove { _processUntilHeader.DataRecieved -= value; } }

		#endregion Events

		#region C-Tor

		public DataReciever()
        {
            _activeProcessor = _processUntilHeader;
            _processUntilHeader.DataRecieved += ProcessingComplete;
            _processUntilFooter.DataRecieved += ProcessingComplete;
		}

		#endregion C-Tor

		#region Event Handlers

		private void ProcessingComplete(byte[] data, int length, bool matchedNewline)
        {
            if (matchedNewline)
                return;

            if (_activeProcessor == _processUntilHeader)
                _activeProcessor = _processUntilFooter;
            else
                _activeProcessor = _processUntilHeader;
        }

		#endregion Event Handlers

		#region Public Methods

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

		#endregion Public Methods
	}
}
