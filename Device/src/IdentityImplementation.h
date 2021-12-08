#ifndef __IDENTITY_IMPL_H__
#define __IDENTITY_IMPL_H__

#include "IdentityDefines.h"
#include "IdentityDevice.h"

/////////////////////////////
// Microcontroller
#if defined( IDENTITY_M_TEENSY32 )
#define IDENTITY_M_VALUE eMicrocontroller::Teensy32
#else
#error "Microcontroller not yet implemented or unsupported"
#endif

/////////////////////////////
// Screen
#if defined( IDENTITY_S_ILI9341 )

#	include <ILI9341_t3.h>
using ScreenApi = ILI9341_t3;

#define IDENTITY_S_VALUE eScreen::ILI9341
#define IDENTITY_R_VALUE eResolution::R240x320

#elif defined( IDENTITY_S_ILI9488 )

#	include <ILI9488_t3.h>

class Banana : public ILI9488_t3
{
public:
	using ILI9488_t3::ILI9488_t3;

	// Return the width of a text string
	// - num =  max characters to process, or 0 for entire string (null-terminated)
	uint16_t measureTextWidth(const char* text, int num = 0) {
		int16_t x1, y1; uint16_t w, h;
		getTextBounds(text, 0, 0, &x1, &y1, &w, &h);
		return w;
	}

	// Return the height of a text string
	// - num =  max characters to process, or 0 for entire string (null-terminated)
	uint16_t measureTextHeight(const char* text, int num = 0) {
		int16_t x1, y1; uint16_t w, h;
		getTextBounds(text, 0, 0, &x1, &y1, &w, &h);
		return h;
	}
};

#define ILI9341_BLACK ILI9488_BLACK
#define ILI9341_BLUE ILI9488_BLUE
#define ILI9341_GREEN ILI9488_GREEN
#define ILI9341_YELLOW ILI9488_YELLOW

using ScreenApi = Banana;// ILI9488_t3;

#define IDENTITY_S_VALUE eScreen::ILI9488
#define IDENTITY_R_VALUE eResolution::R320x480

#else
#error "Screen type not yet implemented or not supported"
#endif

static const Identity Identity{ IDENTITY_M_VALUE, IDENTITY_S_VALUE, IDENTITY_R_VALUE };

#endif // __IDENTITY_IMPL_H__
