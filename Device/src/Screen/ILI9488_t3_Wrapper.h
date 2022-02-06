#ifndef __SCREEN_WRAPPER_ILI9341_H__
#define __SCREEN_WRAPPER_ILI9341_H__

// Include the library
#include <ILI9488_t3.h>

#define ILI9341_WHITE ILI9488_WHITE
#define ILI9341_BLACK ILI9488_BLACK
#define ILI9341_BLUE ILI9488_BLUE
#define ILI9341_GREEN ILI9488_GREEN
#define ILI9341_YELLOW ILI9488_YELLOW

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
