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

#ifndef __SCREEN_WRAPPER_ARDUINO_GFX_H__
#define __SCREEN_WRAPPER_ARDUINO_GFX_H__

// Include the library
#include <Arduino_GFX_Library.h>

#define COLOUR_BLACK       BLACK
#define COLOUR_NAVY        NAVY
#define COLOUR_DARKGREEN   DARKGREEN
#define COLOUR_DARKCYAN    DARKCYAN
#define COLOUR_MAROON      MAROON
#define COLOUR_PURPLE      PURPLE
#define COLOUR_OLIVE       OLIVE
#define COLOUR_LIGHTGREY   LIGHTGREY
#define COLOUR_DARKGREY    DARKGREY
#define COLOUR_BLUE        BLUE
#define COLOUR_GREEN       GREEN
#define COLOUR_CYAN        CYAN
#define COLOUR_RED         RED
#define COLOUR_MAGENTA     MAGENTA
#define COLOUR_YELLOW      YELLOW
#define COLOUR_WHITE       WHITE
#define COLOUR_ORANGE      ORANGE
#define COLOUR_GREENYELLOW GREENYELLOW
#define COLOUR_PINK        PINK

template <typename BusType, typename ScreenType>
class ArduinoGfx
{
public:
	ArduinoGfx();

	void begin() { m_gfx.begin(); }

	void sleep(bool enabled);
	int16_t width() const { return m_gfx.width(); }
	int16_t height() const { return m_gfx.height(); }
	void setRotation(uint8_t rotation) { m_gfx.setRotation(rotation); }
	void fillScreen(uint16_t colour) { m_gfx.fillScreen(colour); }
	void fillRect(int16_t x, int16_t y, int16_t w, int16_t h, uint16_t colour) { m_gfx.fillRect(x, y, w, h, colour); }
	void drawRoundRect(int16_t x0, int16_t y0, int16_t w, int16_t h, int16_t radius, uint16_t color) { m_gfx.drawRoundRect(x0, y0, w, h, radius, color); }
	void drawBitmap(int16_t x, int16_t y, const uint8_t* bitmap, int16_t w, int16_t h, uint16_t color) { m_gfx.drawBitmap(x, y, bitmap, w, h, color, BLACK); }
	void writeRect(int16_t x, int16_t y, int16_t width, int16_t height, const uint16_t* colours) { m_gfx.draw16bitRGBBitmap(x, y, colours, width, height); }

	void setTextColor(uint16_t foreground, uint16_t background) { m_gfx.setTextColor(foreground, background); }
	void setTextColor(uint16_t colour) { m_gfx.setTextColor(colour); }
	void setTextSize(uint8_t size) { m_gfx.setTextSize(size); }
	void setCursor(int16_t x, int16_t y) { m_gfx.setCursor(x, y); }

	void print(const char* text) { m_gfx.print(text); }

	uint16_t measureTextWidth(const char* text)
	{
		int16_t x;
		int16_t y;
		uint16_t h;

		uint16_t width = 0;
		m_gfx.getTextBounds(text, 0, 0, &x, &y, &width, &h);

		return width;
	}

	uint16_t measureTextHeight(const char* text)
	{
		int16_t x;
		int16_t y;
		uint16_t w;

		uint16_t height = 0;
		m_gfx.getTextBounds(text, 0, 0, &x, &y, &w, &height);

		return height;
	}

private:
	BusType m_bus;
	ScreenType m_gfx;
};

template<typename BusType, typename ScreenType>
void ArduinoGfx<BusType, ScreenType>::sleep(bool enabled)
{
	if (enabled)
	{
		// The arduino gfx library seems to remember what was on the screen when we turn it
		// back on, so before turning it off let's wipe it out so it doesn't look buggy
		fillScreen(BLACK);
		m_gfx.displayOff();
	}
	else
	{
		m_gfx.displayOn();
	}
}

#endif // __SCREEN_WRAPPER_ARDUINO_GFX_H__
