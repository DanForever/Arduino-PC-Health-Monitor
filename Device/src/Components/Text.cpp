#include "Text.h"

#include <cstring>

Text::Text()
	: m_longestLength(0)
{
}

void Text::SetPosition(const Position& position)
{
	m_position = position;
}

void Text::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
}

void Text::SetText(Screen* screen, const char* text)
{
	std::strncpy(m_text, text, MAX_LENGTH);

	screen->SetTextSize(m_textSize);

	uint8_t length = strlen(text);

	if (length > m_longestLength)
	{
		m_textWidth = screen->MeasureTextWidth(m_text);
		m_textHeight = screen->MeasureTextHeight(m_text);
		m_longestLength = length;
	}

	m_changed = true;
}

void Text::Draw(Screen* screen)
{
	screen->SetTextSize(m_textSize);
	screen->SetCursor(m_position.X, m_position.Y);
	screen->Print(m_text);

	m_changed = false;
}

void Text::Clear(Screen* screen, uint16_t clearColour)
{
	screen->FillRect(m_position.X, m_position.Y, m_textWidth, m_textHeight, clearColour);
}
