#ifndef __SCREEN_WRAPPER_ILI9341_H__
#define __SCREEN_WRAPPER_ILI9341_H__

// Include the library
#include <ILI9341_t3.h>

#if defined(IDENTITY_M_TEENSY32) || defined(IDENTITY_M_TEENSY40)
#	define SCREEN_TFT_DC      20
#	define SCREEN_TFT_CS      21
#	define SCREEN_TFT_RST     19
#	define SCREEN_TFT_MOSI    11
#	define SCREEN_TFT_SCLK    13
#	define SCREEN_TFT_MISO    12
#else
#	error Platform pins not defined for ILI9341_t3
#endif

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
