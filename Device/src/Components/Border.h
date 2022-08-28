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

#ifndef __COMPONENT_BORDER_H_
#define __COMPONENT_BORDER_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Border final : public Component
{
public:
	Border();

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	int16_t m_borderWidth;
	int16_t m_borderHeight;
	uint16_t m_borderColour;
};

#endif // __COMPONENT_BORDER_H_
