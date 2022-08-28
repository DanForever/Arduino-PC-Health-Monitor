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

#ifndef __MODULE_H_
#define __MODULE_H_

#include "../Components/Component.h"
#include "../Comms.h"

#include <vector>

//------------------------------------------------------------------------------------------------------
class Module
{
public:
	Module(Screen* screen, Position position);
	virtual ~Module();

	void Draw();
	void Clear(uint16_t clearColour = COLOUR_BLACK);

	void SetNumComponents(uint8_t count);
	void AddComponent(Component* component);

	void PushOffset();
	void PopOffset();

	virtual void HandleMessage(Message& message) {}

protected:
	Screen* GetScreen();
	Component* GetComponent(uint8_t index);
	uint8_t GetComponentCount() const { return m_components.size(); }

private:
	Screen* m_screen;
	Position m_position;
	Position m_previousOffset;

	std::vector<Component*> m_components;
};

inline Screen* Module::GetScreen()
{
	return m_screen;
}

inline Component* Module::GetComponent(uint8_t index)
{
	return m_components[index];
}

#endif // __MODULE_H_
