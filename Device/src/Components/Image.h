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

#ifndef __COMPONENT_IMAGE_H_
#define __COMPONENT_IMAGE_H_

#include "Component.h"

class Module;

//------------------------------------------------------------------------------------------------------
class Image final : public Component
{
public:
	Image() = default; // TEMP

	Image(Module* parent);

	void Centre(bool centre);
	void SetPosition(const Position& position);
	void AddSection(Screen* screen, uint16_t width, uint16_t height, int32_t position, const uint16_t* data, uint16_t dataLength);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour);

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	Position m_position;
	Module* m_parent;
	bool m_centre;
};

#endif // __COMPONENT_IMAGE_H_
