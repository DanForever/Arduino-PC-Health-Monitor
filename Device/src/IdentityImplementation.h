#ifndef __IDENTITY_IMPL_H__
#define __IDENTITY_IMPL_H__

#include "IdentityDefines.h"
#include "IdentityDevice.h"

/////////////////////////////
// Microcontroller
#if defined( IDENTITY_M_TEENSY32 )
#	define IDENTITY_M_VALUE eMicrocontroller::Teensy32
#elif defined( IDENTITY_M_TEENSY40 )
#	define IDENTITY_M_VALUE eMicrocontroller::Teensy40
#else
#	error "Microcontroller not yet implemented or unsupported"
#endif

/////////////////////////////
// Screen
#if defined( IDENTITY_S_ILI9341 )
#	include "Screen/ILI9341_t3_Wrapper.h"
#	define IDENTITY_S_VALUE eScreen::ILI9341
#	define IDENTITY_R_VALUE eResolution::R240x320
#elif defined( IDENTITY_S_ILI9488 )
#	include "Screen/ILI9488_t3_Wrapper.h"
#	define IDENTITY_S_VALUE eScreen::ILI9488
#	define IDENTITY_R_VALUE eResolution::R320x480
#elif defined( IDENTITY_S_ILI9486 )
#	define USE_GENERIC_ARDUINO_TFT_LIBRARY
#	include "Screen/Arduino_GFX_Wrapper.h"
#	define IDENTITY_S_VALUE eScreen::ILI9486
#	define IDENTITY_R_VALUE eResolution::R320x480
	using ScreenApi = ArduinoGfx<Arduino_HWSPI, Arduino_ILI9486_18bit>;
#else
#	error "Screen type not yet implemented or not supported"
#endif

static const Identity Identity{ IDENTITY_M_VALUE, IDENTITY_S_VALUE, IDENTITY_R_VALUE };

#endif // __IDENTITY_IMPL_H_
