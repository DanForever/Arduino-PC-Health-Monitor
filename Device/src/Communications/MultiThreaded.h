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

#ifndef __COMMUNICATIONS_MULTITHREADED_H__
#define __COMMUNICATIONS_MULTITHREADED_H__

#include <atomic>
#include <stdint.h>
#include <cstddef>
#include <Arduino.h>
#include <pico/critical_section.h>

#include "Buffer.h"
#include "Message.h"

namespace Communications
{
	static const uint8_t MESSAGE_ARRAY_SIZE = 4;

	struct SectionData
	{
		uint8_t Start = 0;
		uint8_t End = 0;
		uint8_t Count = 0;
		uint8_t Padding = 0;
	};

	class ScopedLock
	{
	public:
		ScopedLock(critical_section_t& criticalSection)
			: m_criticalSection(criticalSection)
		{
			critical_section_enter_blocking(&m_criticalSection);
		}

		~ScopedLock()
		{
			critical_section_exit(&m_criticalSection);
		}

	private:
		critical_section_t& m_criticalSection;
	};

	class QueueSection
	{
	public:
		QueueSection()
		{
			critical_section_init(&m_cs);
		}


		bool Allocate(uint8_t quantity = 1)
		{
			ScopedLock lock(m_cs);

			if (m_data.Count + quantity > MESSAGE_ARRAY_SIZE)
				return false;

			m_data.End = Increment(m_data.End, quantity);
			m_data.Count = m_data.Count + quantity;

			return true;
		}

		bool Release(uint8_t quantity = 1)
		{
			ScopedLock lock(m_cs);

			if (m_data.Count < quantity)
				return false;

			m_data.Start = Increment(m_data.Start, quantity);
			m_data.Count = m_data.Count - quantity;

			return true;
		}

		uint8_t Start() const { ScopedLock lock(m_cs); return m_data.Start; }
		uint8_t End() const { ScopedLock lock(m_cs); return m_data.End; }
		uint8_t Count() const { ScopedLock lock(m_cs); return m_data.Count;}

	private:
		static uint8_t Increment(uint8_t value, uint8_t quantity) { return (value + quantity) % MESSAGE_ARRAY_SIZE; }

		SectionData m_data;
		mutable critical_section_t m_cs;
	};

	class MultiThreaded
	{
	public:
		MultiThreaded();

		void Update();
		void Ack(uint16_t id);
		void SendIdentity();
		void SendVersion();
		void ClearMessage();

		bool MessageReady() const { return m_queue.Count() > 0; }
		Message& GetMessage() { return m_messages[m_queue.Start()]; }

	private:
		void ReadIntoMainBuffer();
		void ProcessMainBuffer();
		bool CouldBePattern(const char* pattern) const;

		Message& GetWritableMessage() { return m_messages[m_queue.End() % MESSAGE_ARRAY_SIZE]; }

	private:
		Buffer<1024 * 10> m_mainBuffer;
		Buffer<16> m_parsingBuffer;

		Message m_messages[MESSAGE_ARRAY_SIZE];
		QueueSection m_queue;

		bool m_lookingForFooter = false;
		bool m_connected = false;
	};
}

#endif // __COMMUNICATIONS_MULTITHREADED_H__
