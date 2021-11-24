#include "FloatWithUnit.h"

#include <cstring>

FloatWithUnit::FloatWithUnit()
	: m_valueWidth(0)
	, m_unitWidth(0)
	, m_textWidth(0)
	, m_textHeight(0)
	, m_textSize(0)
	, m_unitTextSize(0)
{
	m_value[0] = '\0';
	m_unit[0] = '\0';
}

void FloatWithUnit::SetPosition(const Position& position)
{
	m_position = position;
}

void FloatWithUnit::SetTextSize(uint8_t textSize)
{
	m_textSize = textSize;
	m_unitTextSize = m_textSize > 2 ? m_textSize - 2 : 1;
}

void FloatWithUnit::SetValue(Screen* screen, float value)
{
	snprintf(m_value, MAX_VALUE_LENGTH, "%.0f", value);

	screen->SetTextSize(m_textSize);
	m_valueWidth = screen->MeasureTextWidth(m_value);
	m_textHeight = screen->MeasureTextHeight(m_value);

	CalculateTextWidth();

	m_changed = true;
}

void FloatWithUnit::SetUnit(Screen* screen, const char* unit)
{
	std::strncpy(m_unit, unit, MAX_UNIT_LENGTH);

	screen->SetTextSize(m_unitTextSize);
	m_unitWidth = screen->MeasureTextWidth(m_unit);

	CalculateTextWidth();

	m_changed = true;
}

void FloatWithUnit::Draw(Screen* screen)
{
	screen->SetTextSize(m_textSize);
	screen->SetCursor(m_position.X, m_position.Y);
	screen->Print(m_value);
	screen->SetTextSize(m_unitTextSize);
	screen->Print(m_unit);

	m_changed = false;
}

void FloatWithUnit::Clear(Screen* screen, uint16_t clearColour)
{
	screen->FillRect(m_position.X, m_position.Y, m_textWidth, m_textHeight, clearColour);
}

void FloatWithUnit::CalculateTextWidth()
{
	uint16_t textWidth = m_valueWidth + m_unitWidth;

	if (m_textWidth < textWidth)
		m_textWidth = textWidth;
}
