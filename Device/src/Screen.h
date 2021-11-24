#ifndef __SCREEN_H__
#define __SCREEN_H__

#include <ILI9341_t3.h>
#define ILI9341_SILVER      0xA510

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
		FillScreen(ILI9341_BLACK);
	}

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

	void FillScreen(uint16_t color)
	{
		m_tft.fillScreen(color);
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
	ILI9341_t3 m_tft;
	int16_t m_xOffset;
	int16_t m_yOffset;
};

#endif // __SCREEN_H__
