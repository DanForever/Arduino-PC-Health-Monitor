/*
 * Arduino PC Health Monitor (Device firmware)
 * Polls the hardware sensors for data and forwards them on to the arduino device
 * Copyright (C) 2022 Daniel Neve
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

#ifndef __COMMS_H__
#define __COMMS_H__

#include <stdint.h>
#include <cstddef>
#include <Arduino.h>

template <int Capacity>
class Buffer
{
public:
	uint8_t Read()
	{
		uint8_t byte = m_buffer[m_readPosition];
		IncrementReadPosition();
		return byte;
	}

	void Read(char* textBuffer, uint16_t bufferSize)
	{
		const uint16_t messageSize = Size();
		const size_t amountToCopy = min(messageSize, bufferSize);

		for (size_t i = 0; i < amountToCopy; ++i)
		{
			textBuffer[i] = Read();
		}

		uint16_t nullTerminatorPosition = messageSize;
		if (nullTerminatorPosition >= bufferSize)
			nullTerminatorPosition = bufferSize - 1;
		textBuffer[nullTerminatorPosition] = '\0';
	}

	template <typename T>
	uint16_t Read(T* buffer, uint16_t bufferSize)
	{
		uint16_t index = 0;
		while (Size() > 0 && index < bufferSize)
		{
			Read<T>(buffer[index]);
			++index;
		}

		return index;
	}

	template <typename T>
	void Read(T& value)
	{
		uint8_t* output = reinterpret_cast<uint8_t*>(&value);

		for (size_t i = 0; i < sizeof(T); ++i)
		{
			output[i] = Read();
		}
	}

	uint8_t Peek() const
	{
		return m_buffer[m_readPosition];
	}

	bool Write(const uint8_t* source, uint16_t size)
	{
		const uint16_t freeSpace = FreeSpace();
		if (size <= freeSpace)
		{
			for (uint16_t i = 0; i < size; ++i)
			{
				m_buffer[m_writePosition] = source[i];
				IncrementWritePosition();
			}

			return true;
		}

		return false;
	}

	template <typename T>
	void Write(const T& value)
	{
		const uint8_t* input = reinterpret_cast<const uint8_t*>(&value);

		Write(input, (uint16_t)sizeof(T));
	}

	bool Write(uint8_t byte) { return Write(&byte, 1); }

	bool operator<<(uint8_t byte) { return Write(byte); }

	template <int T>
	void operator<<(Buffer<T>& buffer)
	{
		while (buffer.Size() > 0)
		{
			Write(buffer.Read());
		}
	}

	uint8_t operator[](uint16_t index) const
	{
		uint16_t position = (m_readPosition + index) % Capacity;
		return m_buffer[position];
	}

	uint16_t FreeSpace() const
	{
		return Capacity - m_size;
	}

	uint16_t Size() const { return m_size; }

	const uint8_t* Raw() const { return &m_buffer[m_writePosition]; }

	void Reset()
	{
		m_readPosition = 0;
		m_writePosition = 0;
		m_size = 0;
	}

	void DebugPrint() const
	{
		int size = Size();
		for (int i = 0; i < size; ++i)
		{
			int position = (m_readPosition + i) % Capacity;
			char value = m_buffer[position];
			Serial.print(value);
		}
		Serial.println("");
	}

private:
	static void IncrementPosition(uint16_t& position)
	{
		++position;
		if (position >= Capacity)
			position = 0;
	}

	void IncrementWritePosition()
	{
		IncrementPosition(m_writePosition);
		++m_size;
	}

	void IncrementReadPosition()
	{
		IncrementPosition(m_readPosition);
		--m_size;
	}

	uint16_t m_readPosition = 0;
	uint16_t m_writePosition = 0;
	uint16_t m_size = 0;
	uint8_t m_buffer[Capacity];
};

using Message = Buffer<1024 * 2>;

class Comms
{
public:
	Comms();

	void Update();
	void Ack(uint16_t id);
	void SendIdentity();
	void SendVersion();
	void ClearMessage();

	bool MessageReady() { return m_messageReady; }
	Message& GetMessage() { return m_outputBuffer; }

private:
	void ReadIntoMainBuffer();
	void ProcessMainBuffer();
	bool CouldBePattern(const char* pattern) const;

private:
	Buffer<1024 * 10> m_mainBuffer;
	Buffer<16> m_parsingBuffer;
	Message m_outputBuffer;

	bool m_messageReady;
	bool m_lookingForFooter;
	bool m_connected;
};

#endif // __COMMS_H__
