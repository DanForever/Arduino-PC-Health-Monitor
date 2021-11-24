#include "ClockSpeed.h"

ClockSpeed::ClockSpeed()
	: m_textWidth(0)
	, m_textHeight(0)
{
}

void ClockSpeed::SetPosition(const Position& position)
{
	m_position = position;
}

void ClockSpeed::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
	m_unitTextSize = m_textSize > 3 ? m_textSize - 3 : 1;
}

void ClockSpeed::SetValue(Screen* screen, float value)
{
	snprintf(m_text, MAX_LENGTH, "%.0f", value);

	screen->SetTextSize(m_textSize);
	uint16_t width = screen->MeasureTextWidth(m_text);
	m_textHeight = screen->MeasureTextHeight(m_text);

	screen->SetTextSize(m_unitTextSize);
	width += screen->MeasureTextWidth("Mhz");

	if (m_textWidth < width)
		m_textWidth = width;

	m_changed = true;
}

void ClockSpeed::Draw(Screen* screen)
{
	screen->SetTextSize(m_textSize);
	screen->SetCursor(m_position.X, m_position.Y);
	screen->Print(m_text);
	screen->SetTextSize(m_unitTextSize);
	screen->Print("Mhz");

	m_changed = false;
}

void ClockSpeed::Clear(Screen* screen, uint16_t clearColour)
{
	screen->FillRect(m_position.X, m_position.Y, m_textWidth, m_textHeight, clearColour);
}
