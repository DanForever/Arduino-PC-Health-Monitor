#include "Image.h"

Image::Image()
	: m_centre(false)
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

void Image::Draw(Screen*)
{
}

void Image::Clear(Screen* screen, uint16_t clearColour)
{
}
