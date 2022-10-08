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

#ifndef __COMPONENT_FLOAT_WITH_UNIT_H_
#define __COMPONENT_FLOAT_WITH_UNIT_H_

#include "Component.h"
#include "../Screen/PrintUtils.h"

//------------------------------------------------------------------------------------------------------
class FloatWithUnit final : public Component
{
public:
	static const uint8_t MAX_VALUE_LENGTH = 8;
	static const uint8_t MAX_UNIT_LENGTH = 4;

	FloatWithUnit();

	void SetPosition(const Position& position);
	void SetTextSize(uint8_t textSize);
	void SetValue(Screen* screen, float value);
	void SetUnit(Screen* screen, const char* unit);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	int m_precision;

	Printer m_printer;

	Position m_position;
	uint8_t m_textSize;
	uint8_t m_unitTextSize;

	char m_value[MAX_VALUE_LENGTH];
	char m_unit[MAX_UNIT_LENGTH];
};

#endif // __COMPONENT_FLOAT_WITH_UNIT_H_
