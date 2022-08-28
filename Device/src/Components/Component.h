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

#ifndef __COMPONENT_H__
#define __COMPONENT_H__

#include "../Screen/Screen.h"
#include "../Comms.h"

struct Position
{
	int16_t X;
	int16_t Y;
};

//------------------------------------------------------------------------------------------------------
class Component
{
public:
	Component() : m_changed(false) {}
	virtual ~Component() = default;

	virtual void Draw(Screen* screen) = 0;
	virtual void Clear(Screen* screen, uint16_t clearColour) = 0;

	virtual void HandleSetupMessage(Screen* screen, Message& message) {}
	virtual void HandleUpdateMessage(Screen* screen, Message& message) {}

	bool HasChanged() const { return m_changed; }

protected:
	bool m_changed;
};


#endif // __COMPONENT_H__
