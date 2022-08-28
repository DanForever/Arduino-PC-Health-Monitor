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

#include "PrintUtils.h"

Printer::Printer(Screen* screen)
	: m_screen(screen)
{}

void Printer::Print(const char* text, int16_t x, int16_t y, const Settings& settings)
{
	Dimensions old = m_previousTextDimensions;

	// Alias the variable for better readability
	Dimensions& current = m_previousTextDimensions;
	current.Calculate(text, settings, m_screen);
	current.Position.X = x;
	current.Position.Y = y;

	if (current.Width < old.Width)
	{
		{
			// Clear left side
			int16_t left = old.Position.X + old.Offset.X;
			int16_t right = current.Position.X + current.Offset.X;
			int16_t top = old.Position.Y + old.Offset.Y;
			int16_t bottom = top + old.Height;

			m_screen->FillRect(left, top, right - left, bottom - top, settings.Background);
		}

		{
			// Clear right side
			int16_t left = current.Position.X + current.Offset.X + current.Width;
			int16_t right = old.Position.X + old.Offset.X + old.Width;
			int16_t top = old.Position.Y + old.Offset.Y;
			int16_t bottom = top + old.Height;

			m_screen->FillRect(left, top, right - left, bottom - top, settings.Background);
		}
	}

	// @todo: Check to see if the new text is shorter and repeat the code block but for the Y dimension

	m_screen->SetTextSize(settings.TextSize);
	m_screen->SetTextColour(settings.Foreground, settings.Background);
	m_screen->SetCursor(current.Position.X + current.Offset.X, current.Position.Y + current.Offset.Y);
	m_screen->Print(text);
}

void Printer::Dimensions::Calculate(const char* text, const Settings& settings, Screen* screen)
{
	screen->SetTextSize(settings.TextSize);

	Width = screen->MeasureTextWidth(text);
	Height = screen->MeasureTextHeight(text);

	switch (settings.Horizontal)
	{
	case HorizontalAlignment::Left:
		Offset.X = 0;
		break;

	case HorizontalAlignment::Centre:
		Offset.X = -Width / 2;
		break;

	case HorizontalAlignment::Right:
		Offset.X = -Width;
		break;
	}

	switch (settings.Vertical)
	{
	case VerticalAlignment::Top:
		Offset.Y = 0;
		break;

	case VerticalAlignment::Centre:
		Offset.Y = -Height / 2;
		break;

	case VerticalAlignment::Bottom:
		Offset.Y = -Height;
		break;
	}
}
