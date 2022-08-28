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

#include "Border.h"

Border::Border() = default;

void Border::Draw(Screen* screen)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, m_borderColour);

	m_changed = false;
}

void Border::Clear(Screen* screen, uint16_t clearColour)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, clearColour);
}

void Border::HandleSetupMessage(Screen* screen, Message& message)
{
	uint16_t dummy;
	message.Read(dummy);
	message.Read(dummy);

	message.Read(m_borderWidth);
	message.Read(m_borderHeight);
	message.Read(m_borderColour);

	m_changed = true;
}

void Border::HandleUpdateMessage(Screen* screen, Message& message)
{
}
