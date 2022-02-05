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

	uint16_t Foreground = ILI9341_WHITE;
	uint16_t Background = ILI9341_BLACK;
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
