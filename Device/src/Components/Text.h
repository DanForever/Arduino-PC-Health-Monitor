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

#ifndef __COMPONENT_TEXT_H_
#define __COMPONENT_TEXT_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Text : public Component
{
public:
	static const uint8_t MAX_LENGTH = 32;

	Text();

	void SetPosition(const Position& position);
	void SetTextSize(uint8_t textSize);
	void SetText(Screen* screen, const char* text);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	uint16_t m_textWidth;
	uint16_t m_textHeight;

	Position m_position;
	uint8_t m_textSize;
	uint8_t m_longestLength;
	char m_text[MAX_LENGTH];
};

#endif // __COMPONENT_TEXT_H_
