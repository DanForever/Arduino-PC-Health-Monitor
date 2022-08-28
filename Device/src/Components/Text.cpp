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

#include "Text.h"

#include <cstring>

Text::Text()
	: m_longestLength(0)
{
}

void Text::SetPosition(const Position& position)
{
	m_position = position;
}

void Text::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
}

void Text::SetText(Screen* screen, const char* text)
{
	std::strncpy(m_text, text, MAX_LENGTH);

	screen->SetTextSize(m_textSize);

	uint8_t length = strlen(text);

	if (length > m_longestLength)
	{
		m_textWidth = screen->MeasureTextWidth(m_text);
		m_textHeight = screen->MeasureTextHeight(m_text);
		m_longestLength = length;
	}

	m_changed = true;
}

void Text::Draw(Screen* screen)
{
	screen->SetTextSize(m_textSize);
	screen->SetCursor(m_position.X, m_position.Y);
	screen->Print(m_text);

	m_changed = false;
}

void Text::Clear(Screen* screen, uint16_t clearColour)
{
	screen->FillRect(m_position.X, m_position.Y, m_textWidth, m_textHeight, clearColour);
}

void Text::HandleSetupMessage(Screen* screen, Message& message)
{
	// Format:
	//
	// int16_t Position X
	// int16_t Position Y
	//
	// uint8_t text size

	message.Read(m_position.X);
	message.Read(m_position.Y);

	message.Read(m_textSize);
}

void Text::HandleUpdateMessage(Screen* screen, Message& message)
{
	char buffer[Text::MAX_LENGTH];
	message.Read(buffer, Text::MAX_LENGTH);

	SetText(screen, buffer);
}
