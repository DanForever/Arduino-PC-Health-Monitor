#include "Comms.h"

#include <Arduino.h>
#include "PacketType.h"
#include "IdentityImplementation.h"

namespace
{
	const char* MessageHeader = "dan<`";
	const char* MessageFooter = "`>dan";
}

Comms::Comms()
	: m_messageReady(false)
	, m_lookingForFooter(false)
	, m_connected(false)
{
	Serial.begin(9600);
}

void Comms::Update()
{
	ReadIntoMainBuffer();
	ProcessMainBuffer();
}

void Comms::Ack(uint16_t id)
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

bool Comms::CouldBePattern(const char* pattern) const
{
	for (int8_t i = 0; i < m_parsingBuffer.Size(); ++i)
	{
		if (m_parsingBuffer[i] != pattern[i])
			return false;
	}

	return true;
}

void Comms::ClearMessage()
{
	m_messageReady = false;

	m_outputBuffer.Reset();
}

void Comms::ReadIntoMainBuffer()
{
	while (::Serial.available())
	{
		m_mainBuffer << ::Serial.read();
	}
}

void Comms::ProcessMainBuffer()
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

void Comms::SendIdentity()
{
	uint8_t buffer[8];

	buffer[0] = (uint8_t)ePacketType::Identity;
	buffer[1] = (uint8_t)Identity.Microcontroller;
	buffer[2] = (uint8_t)Identity.Screen;
	buffer[3] = (uint8_t)Identity.Resolution;

	::Serial.write(MessageHeader, strlen(MessageHeader));
	::Serial.write(buffer, 4);
	::Serial.write(MessageFooter, strlen(MessageFooter));
}
