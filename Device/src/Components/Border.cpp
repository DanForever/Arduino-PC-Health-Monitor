#include "Border.h"

Border::Border(int16_t width, int16_t height, uint16_t colour)
	: m_borderWidth(width)
	, m_borderHeight(height)
	, m_borderColour(colour)
{
	m_changed = true;
}

void Border::Draw(Screen* screen)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, m_borderColour);

	m_changed = false;
}

void Border::Clear(Screen* screen, uint16_t clearColour)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, clearColour);
}
