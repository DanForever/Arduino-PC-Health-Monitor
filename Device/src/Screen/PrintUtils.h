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

#ifndef __PRINT_UTILS_H__
#define __PRINT_UTILS_H__

#include "Screen.h"

enum class HorizontalAlignment : uint8_t
{
	Left,
	Centre,
	Right
};

enum class VerticalAlignment : uint8_t
{
	Top,
	Centre,
	Bottom
};

struct Settings
{
	uint8_t TextSize = 1;

	HorizontalAlignment Horizontal = HorizontalAlignment::Left;
	VerticalAlignment Vertical = VerticalAlignment::Top;

	uint16_t Foreground = COLOUR_WHITE;
	uint16_t Background = COLOUR_BLACK;
};

// This is a special helper class designed to reduce flicker when printing string values to the screen
// It does this in 2 ways:
// 1: Print the new text using a black background (so any old text is overwritten)
// 2: If the new text is smaller than the old text, then clear out the old text to the sides of the new text
//   (it does this by storing the dimensions of the last string printed)
class Printer
{
private:
	struct Dimensions
	{
		// @todo: Consolidate with the "Position" struct in Component.h
		struct Point
		{
			int16_t X;
			int16_t Y;
		};

		uint16_t Width;
		uint16_t Height;

		Point Position;
		Point Offset;

		void Calculate(const char* text, const Settings& settings, Screen* screen);
	};

public:
	Printer(Screen* screen);

	void Print(const char* text, int16_t x, int16_t y, const Settings& settings = {});

private:
	Screen* m_screen;

	Dimensions m_previousTextDimensions;
};

#endif // __PRINT_UTILS_H__
