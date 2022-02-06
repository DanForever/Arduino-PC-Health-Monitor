#ifndef __SCREEN_WRAPPER_ILI9341_H__
#define __SCREEN_WRAPPER_ILI9341_H__

// Include the library
#include <ILI9488_t3.h>

#if defined(IDENTITY_M_TEENSY32) || defined(IDENTITY_M_TEENSY40)
#	define SCREEN_TFT_DC      20
#	define SCREEN_TFT_CS      21
#	define SCREEN_TFT_RST     19
#	define SCREEN_TFT_MOSI    11
#	define SCREEN_TFT_SCLK    13
#	define SCREEN_TFT_MISO    12
#else
#	error Platform pins not defined for ILI9488_t3
#endif

#define COLOUR_BLACK       ILI9488_BLACK
#define COLOUR_NAVY        ILI9488_NAVY
#define COLOUR_DARKGREEN   ILI9488_DARKGREEN
#define COLOUR_DARKCYAN    ILI9488_DARKCYAN
#define COLOUR_MAROON      ILI9488_MAROON
#define COLOUR_PURPLE      ILI9488_PURPLE
#define COLOUR_OLIVE       ILI9488_OLIVE
#define COLOUR_LIGHTGREY   ILI9488_LIGHTGREY
#define COLOUR_DARKGREY    ILI9488_DARKGREY
#define COLOUR_BLUE        ILI9488_BLUE
#define COLOUR_GREEN       ILI9488_GREEN
#define COLOUR_CYAN        ILI9488_CYAN
#define COLOUR_RED         ILI9488_RED
#define COLOUR_MAGENTA     ILI9488_MAGENTA
#define COLOUR_YELLOW      ILI9488_YELLOW
#define COLOUR_WHITE       ILI9488_WHITE
#define COLOUR_ORANGE      ILI9488_ORANGE
#define COLOUR_GREENYELLOW ILI9488_GREENYELLOW
#define COLOUR_PINK        ILI9488_PINK

class ScreenApi : public ILI9488_t3
{
public:
	ScreenApi()
		: ILI9488_t3(SCREEN_TFT_CS, SCREEN_TFT_DC, SCREEN_TFT_RST, SCREEN_TFT_MOSI, SCREEN_TFT_SCLK, SCREEN_TFT_MISO)
	{
	}

	// Return the width of a text string
	// - num =  max characters to process, or 0 for entire string (null-terminated)
	uint16_t measureTextWidth(const char* text, int num = 0)
	{
		int16_t x1, y1; uint16_t w, h;
		getTextBounds(text, 0, 0, &x1, &y1, &w, &h);
		return w;
	}

	// Return the height of a text string
	// - num =  max characters to process, or 0 for entire string (null-terminated)
	uint16_t measureTextHeight(const char* text, int num = 0)
	{
		int16_t x1, y1; uint16_t w, h;
		getTextBounds(text, 0, 0, &x1, &y1, &w, &h);
		return h;
	}
private:
};

#endif // __SCREEN_WRAPPER_ILI9341_H__
