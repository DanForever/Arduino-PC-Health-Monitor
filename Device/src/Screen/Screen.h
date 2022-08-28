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

#ifndef __SCREEN_H__
#define __SCREEN_H__

#include "../IdentityImplementation.h"

class Screen
{
public:
	enum Rotation
	{
		Zero,
		Ninety,
		OneEighty,
		TwoSeventy,
	};

	Screen();

	void Initialize()
	{
		m_tft.begin();
		FillScreen(COLOUR_BLACK);
	}

	void Sleep()
	{
		m_tft.sleep(true);
	}

	void Wakeup()
	{
		m_tft.sleep(false);
	}

	int16_t Width() const { return m_tft.width(); }
	int16_t Height() const { return m_tft.height(); }

	void SetOffset(int16_t x, int16_t y)
	{
		m_xOffset = x;
		m_yOffset = y;
	}

	void GetOffset(int16_t& x, int16_t y) const
	{
		x = m_xOffset;
		y = m_yOffset;
	}

	void ClearOffset()
	{
		m_xOffset = 0;
		m_yOffset = 0;
	}

	void SetRotation(Rotation rotation)
	{
		m_tft.setRotation((int)rotation);
	}

	void FillScreen(uint16_t colour)
	{
		m_tft.fillScreen(colour);
	}

	void FillRect(int16_t x, int16_t y, int16_t w, int16_t h, uint16_t colour)
	{
		m_tft.fillRect(x + m_xOffset, y + m_yOffset, w, h, colour);
	}

	void DrawRoundRect(int16_t x0, int16_t y0, int16_t w, int16_t h, int16_t radius, uint16_t color)
	{
		m_tft.drawRoundRect(x0 + m_xOffset, y0 + m_yOffset, w, h, radius, color);
	}

	void DrawBitmap(int16_t x, int16_t y, const uint8_t* bitmap, int16_t w, int16_t h, uint16_t color)
	{
		m_tft.drawBitmap(x + m_xOffset, y + m_yOffset, bitmap, w, h, color);
	}

	void WriteRect(int16_t x, int16_t y, int16_t width, int16_t height, const uint16_t* colours)
	{
		m_tft.writeRect(x + m_xOffset, y + m_yOffset, width, height, colours);
	}

	void SetTextColour(uint16_t foreground, uint16_t background) { m_tft.setTextColor(foreground, background); }
	void SetTextColour(uint16_t colour) { m_tft.setTextColor(colour); }
	void SetTextSize(uint8_t size) { m_tft.setTextSize(size); }
	void SetCursor(int16_t x, int16_t y)
	{
		m_tft.setCursor(x + m_xOffset, y + m_yOffset);
	}

	void Print(const char* text)
	{
		m_tft.print(text);
	}

	uint16_t MeasureTextWidth(const char* text) { return m_tft.measureTextWidth(text); }
	uint16_t MeasureTextHeight(const char* text) { return m_tft.measureTextHeight(text); }

private:
	mutable ScreenApi m_tft;
	int16_t m_xOffset;
	int16_t m_yOffset;
};

#endif // __SCREEN_H__
