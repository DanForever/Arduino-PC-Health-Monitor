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

#ifndef __SCREEN_WRAPPER_ILI9341_H__
#define __SCREEN_WRAPPER_ILI9341_H__

// Include the library
#include <ILI9341_t3.h>

#define COLOUR_BLACK       ILI9341_BLACK
#define COLOUR_NAVY        ILI9341_NAVY
#define COLOUR_DARKGREEN   ILI9341_DARKGREEN
#define COLOUR_DARKCYAN    ILI9341_DARKCYAN
#define COLOUR_MAROON      ILI9341_MAROON
#define COLOUR_PURPLE      ILI9341_PURPLE
#define COLOUR_OLIVE       ILI9341_OLIVE
#define COLOUR_LIGHTGREY   ILI9341_LIGHTGREY
#define COLOUR_DARKGREY    ILI9341_DARKGREY
#define COLOUR_BLUE        ILI9341_BLUE
#define COLOUR_GREEN       ILI9341_GREEN
#define COLOUR_CYAN        ILI9341_CYAN
#define COLOUR_RED         ILI9341_RED
#define COLOUR_MAGENTA     ILI9341_MAGENTA
#define COLOUR_YELLOW      ILI9341_YELLOW
#define COLOUR_WHITE       ILI9341_WHITE
#define COLOUR_ORANGE      ILI9341_ORANGE
#define COLOUR_GREENYELLOW ILI9341_GREENYELLOW
#define COLOUR_PINK        ILI9341_PINK

class ScreenApi : public ILI9341_t3
{
public:
	ScreenApi()
		: ILI9341_t3(SCREEN_TFT_CS, SCREEN_TFT_DC, SCREEN_TFT_RST, SCREEN_TFT_MOSI, SCREEN_TFT_SCLK, SCREEN_TFT_MISO)
	{
	}

private:
};

#endif // __SCREEN_WRAPPER_ILI9341_H__
