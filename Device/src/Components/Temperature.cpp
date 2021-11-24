#include "Temperature.h"

void Temperature::SetPosition(const Position& position)
{
	m_position = position;
}

void Temperature::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
	m_unitTextSize = m_textSize > 2 ? m_textSize - 2 : 1;
}

void Temperature::SetValue(Screen* screen, float value)
{
	snprintf(m_text, MAX_LENGTH, "%.0f", value);

	screen->SetTextSize(m_textSize);
	m_textWidth = screen->MeasureTextWidth(m_text);
	m_textHeight = screen->MeasureTextHeight(m_text);

	screen->SetTextSize(m_unitTextSize);
	m_textWidth += screen->MeasureTextWidth("C");

	m_changed = true;
}

void Temperature::Draw(Screen* screen)
{
	screen->SetTextSize(m_textSize);
	screen->SetCursor(m_position.X, m_position.Y);
	screen->Print(m_text);
	screen->SetTextSize(m_unitTextSize);
	screen->Print("C");

	m_changed = false;
}

void Temperature::Clear(Screen* screen, uint16_t clearColour)
{
	screen->FillRect(m_position.X, m_position.Y, m_textWidth, m_textHeight, clearColour);
}
