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

#ifndef __COMMUNICATIONS_SINGLETHREADED_H__
#define __COMMUNICATIONS_SINGLETHREADED_H__

#include <stdint.h>
#include <cstddef>
#include <Arduino.h>

#include "Buffer.h"
#include "Message.h"

namespace Communications
{
	class SingleThreaded
	{
	public:
		SingleThreaded();

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
}

#endif // __COMMUNICATIONS_SINGLETHREADED_H__
