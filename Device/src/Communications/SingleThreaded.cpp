// Arduino PC Health Monitor (Device firmware)
// Polls the hardware sensors for data and forwards them on to the arduino device
// Copyright (C) 2022 Daniel Neve
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#include "SingleThreaded.h"

#include <Arduino.h>

#include "PacketType.h"

#include "../Version.h"
#include "../IdentityImplementation.h"

namespace
{
	const char* MessageHeader = "dan<`";
	const char* MessageFooter = "`>dan";
}

Communications::SingleThreaded::SingleThreaded()
	: m_messageReady(false)
	, m_lookingForFooter(false)
	, m_connected(false)
{
	Serial.begin(9600);
}

void Communications::SingleThreaded::Update()
{
	ReadIntoMainBuffer();
	ProcessMainBuffer();
}

void Communications::SingleThreaded::Ack(uint16_t id)
{
	// @todo: Don't magic number the length, use a proper (ideally compile time) way of computing the needed length
	//uint8_t buffer[5 + 5 + 1 + 2];
	uint8_t buffer[128];

	size_t bufferUsed = 0;
	size_t headerLength = strlen(MessageHeader);
	memcpy(buffer, MessageHeader, headerLength);
	bufferUsed += headerLength;

	buffer[bufferUsed] = (uint8_t)ePacketType::GaranteedAck;
	++bufferUsed;

	memcpy(buffer + bufferUsed, &id, sizeof(uint16_t));
	bufferUsed += sizeof(uint16_t);

	size_t footerLength = strlen(MessageFooter);
	memcpy(buffer + bufferUsed, MessageFooter, footerLength);
	bufferUsed += footerLength;

	::Serial.write(buffer, bufferUsed);
}

bool Communications::SingleThreaded::CouldBePattern(const char* pattern) const
{
	for (int8_t i = 0; i < m_parsingBuffer.Size(); ++i)
	{
		if (m_parsingBuffer[i] != pattern[i])
			return false;
	}

	return true;
}

void Communications::SingleThreaded::ClearMessage()
{
	m_messageReady = false;

	m_outputBuffer.Reset();
}

void Communications::SingleThreaded::ReadIntoMainBuffer()
{
	while (::Serial.available())
	{
		m_mainBuffer << ::Serial.read();
	}
}

void Communications::SingleThreaded::ProcessMainBuffer()
{
	while (m_mainBuffer.Size() > 0)
	{
		m_parsingBuffer << m_mainBuffer.Read();

		const bool m_lookingForHeader = !m_lookingForFooter;
		if (m_lookingForHeader)
		{
			if (CouldBePattern(MessageHeader))
			{
				if (m_parsingBuffer.Size() == strlen(MessageHeader))
				{
					// Discard the header, we don't need it
					m_parsingBuffer.Reset();

					m_lookingForFooter = true;
				}
			}
			else
			{
				//There shouldn't be any data after an end and before a start
				// but if there is, discard it
				m_parsingBuffer.Reset();
			}
		}

		if (m_lookingForFooter)
		{
			if (!CouldBePattern(MessageFooter))
			{
				m_outputBuffer << m_parsingBuffer;
			}
			else if (m_parsingBuffer.Size() == strlen(MessageFooter))
			{
				// Discard the footer, we don't need it
				m_parsingBuffer.Reset();

				m_messageReady = true;
				m_lookingForFooter = false;

				break;
			}
		}
	}
}

void Communications::SingleThreaded::SendIdentity()
{
	uint8_t buffer[4];

	buffer[0] = (uint8_t)ePacketType::Identity;
	buffer[1] = (uint8_t)Identity.Microcontroller;
	buffer[2] = (uint8_t)Identity.Screen;
	buffer[3] = (uint8_t)Identity.Resolution;

	::Serial.write(MessageHeader, strlen(MessageHeader));
	::Serial.write(buffer, 4);
	::Serial.write(MessageFooter, strlen(MessageFooter));
}

void Communications::SingleThreaded::SendVersion()
{
	uint8_t buffer[4];
	
	buffer[0] = (uint8_t)ePacketType::Version;
	buffer[1] = VERSION_MAJOR;
	buffer[2] = VERSION_MINOR;
	buffer[3] = VERSION_PATCH;

	::Serial.write(MessageHeader, strlen(MessageHeader));
	::Serial.write(buffer, 4);
	::Serial.write(MessageFooter, strlen(MessageFooter));
}
