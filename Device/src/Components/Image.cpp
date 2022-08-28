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

#include "Image.h"

#include "../Modules/Module.h"

Image::Image(Module* parent)
	: m_parent(parent)
	, m_centre(false)
{}

void Image::Centre(bool centre)
{
	m_centre = centre;
}

void Image::SetPosition(const Position& position)
{
	m_position = position;
}

void Image::AddSection(Screen* screen, uint16_t width, uint16_t height, int32_t position, const uint16_t* data, uint16_t dataLength)
{
	uint16_t xOffset = m_centre ? -(width / 2) : 0;
	uint16_t yOffset = m_centre ? -(height / 2) : 0;

	while (dataLength > 0)
	{
		uint16_t x = position % width;
		uint16_t y = position / width;

		uint16_t amountOfRowUndrawn = width - x;
		uint16_t lengthToDraw = min(amountOfRowUndrawn, dataLength);

		screen->WriteRect(x + m_position.X + xOffset, y + m_position.Y + yOffset, lengthToDraw, 1, data);

		position += lengthToDraw;
		data += lengthToDraw;
		dataLength -= lengthToDraw;
	}
}

void Image::HandleSetupMessage(Screen* screen, Message& message)
{
	Centre(true);

	message.Read(m_position.X);
	message.Read(m_position.Y);
}

void Image::HandleUpdateMessage(Screen* screen, Message& message)
{
	uint16_t width = -1;
	message.Read(width);

	uint16_t height = -1;
	message.Read(height);

	int32_t position = -1;
	message.Read(position);

	uint16_t buffer[512];
	uint16_t elementsWritten = message.Read(buffer, 512);

	m_parent->PushOffset();
		AddSection(screen, width, height, position / sizeof(uint16_t), buffer, elementsWritten);
	m_parent->PopOffset();
}

void Image::Draw(Screen*)
{
}

void Image::Clear(Screen* screen, uint16_t clearColour)
{
}