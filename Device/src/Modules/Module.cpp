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

#include "Module.h"

Module::Module(Screen* screen, Position position)
	: m_screen(screen)
	, m_position(position)
{}

Module::~Module()
{
	for (auto* component : m_components)
	{
		delete component;
	}

	m_components.clear();
}

void Module::Draw()
{
	PushOffset();

	for (auto* component : m_components)
	{
		if (component->HasChanged())
		{
			component->Clear(GetScreen(), COLOUR_BLACK);
			component->Draw(GetScreen());
		}
	}

	PopOffset();
}

void Module::Clear(uint16_t clearColour)
{
	PushOffset();

	for (auto* component : m_components)
	{
		component->Clear(GetScreen(), clearColour);
	}

	PopOffset();
}

void Module::SetNumComponents(uint8_t count)
{
	m_components.reserve(count);
}

void Module::AddComponent(Component* component)
{
	m_components.push_back(component);
}

void Module::PushOffset()
{
	GetScreen()->GetOffset(m_previousOffset.X, m_previousOffset.Y);
	GetScreen()->SetOffset(m_position.X, m_position.Y);
}

void Module::PopOffset()
{
	GetScreen()->SetOffset(m_previousOffset.X, m_previousOffset.Y);
}
