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

#include "FloatWithUnit.h"

#include <Arduino.h>
#include <cstring>

const char UnitStrCelcius[] PROGMEM = "C";
const char UnitStrMhz[] PROGMEM = "Mhz";
const char UnitStrGB[] PROGMEM = "GB";
const char UnitStrPercent[] PROGMEM = "%";
const char UnitStrFPS[] PROGMEM = "FPS";
const char UnitStrMS[] PROGMEM = "ms";

const char* const UnitStrings[] PROGMEM =
{
	UnitStrCelcius,
	UnitStrMhz,
	UnitStrGB,
	UnitStrPercent,
	UnitStrFPS,
	UnitStrMS,
};

FloatWithUnit::FloatWithUnit()
	: m_precision(0)
	, m_textSize(0)
	, m_unitTextSize(0)
{
	m_value[0] = '\0';
	m_unit[0] = '\0';
}

void FloatWithUnit::SetPosition(const Position& position)
{
	m_position = position;
}

void FloatWithUnit::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
	m_unitTextSize = m_textSize > 2 ? m_textSize - 2 : 1;
}

void FloatWithUnit::SetValue(Screen*, float value)
{
	snprintf(m_value, MAX_VALUE_LENGTH, "%.*f", m_precision, value);

	m_changed = true;
}

void FloatWithUnit::SetUnit(Screen*, const char* unit)
{
	std::strncpy(m_unit, unit, MAX_UNIT_LENGTH);

	m_changed = true;
}

void FloatWithUnit::Draw(Screen* screen)
{
	m_printer.Print(m_value, m_unit, m_position.X, m_position.Y, screen, { m_textSize, m_unitTextSize });

	m_changed = false;
}

void FloatWithUnit::Clear(Screen*, uint16_t)
{
}

void FloatWithUnit::HandleSetupMessage(Screen* screen, Message& message)
{
	// Format:
	//
	// int16_t Position X
	// int16_t Position Y
	//
	// uint8_t text size
	// uint8_t unit text size
	//
	// uint8_t unit
	// uint8_t precision


	message.Read(m_position.X);
	message.Read(m_position.Y);

	message.Read(m_textSize);
	message.Read(m_unitTextSize);

	uint8_t unit;
	message.Read(unit);
	strcpy_P(m_unit, (char*)pgm_read_dword(&(UnitStrings[unit])));

	// The precision is only a byte in the buffer, but printf needs an int in the parameter
	m_precision = message.Read();

	m_changed = true;
}

void FloatWithUnit::HandleUpdateMessage(Screen* screen, Message& message)
{
	float value;
	message.Read(value);
	SetValue(screen, value);
}
