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

#ifndef __IDENTITY_IMPL_H__
#define __IDENTITY_IMPL_H__

#include "IdentityDefines.h"
#include "IdentityDevice.h"

/////////////////////////////
// Microcontroller
#if defined( IDENTITY_M_TEENSY32 )
#	define IDENTITY_M_VALUE eMicrocontroller::Teensy32

#	define SCREEN_TFT_DC      20
#	define SCREEN_TFT_CS      21
#	define SCREEN_TFT_RST     19
#	define SCREEN_TFT_MOSI    11
#	define SCREEN_TFT_SCLK    13
#	define SCREEN_TFT_MISO    12

#	define PIN_BACKLIGHT      8

#	define IDENTITY_M_TEENSY_ANY
#elif defined( IDENTITY_M_TEENSY40 )
#	define IDENTITY_M_VALUE eMicrocontroller::Teensy40

#	define SCREEN_TFT_DC      20
#	define SCREEN_TFT_CS      21
#	define SCREEN_TFT_RST     19
#	define SCREEN_TFT_MOSI    11
#	define SCREEN_TFT_SCLK    13
#	define SCREEN_TFT_MISO    12

#	define PIN_BACKLIGHT      8

#	define IDENTITY_M_TEENSY_ANY
#elif defined( IDENTITY_M_SEEEDUINO_XAIO_RP2040)
#	define IDENTITY_M_VALUE eMicrocontroller::SeeediunoXiaoRp2040

#	define SCREEN_TFT_DC      D1
#	define SCREEN_TFT_CS      D2
#	define SCREEN_TFT_RST     D0
#	define SCREEN_TFT_MOSI    MOSI
#	define SCREEN_TFT_SCLK    SCK
#	define SCREEN_TFT_MISO    MISO

#	define PIN_BACKLIGHT      D3

#	define IDENTITY_M_SEEEDUINO_XAIO_ANY
#else
#	error "Microcontroller not yet implemented or unsupported"
#endif

/////////////////////////////
// Screen
#if defined( IDENTITY_S_ILI9341 )
#	define IDENTITY_S_VALUE eScreen::ILI9341
#	define IDENTITY_R_VALUE eResolution::R240x320
#elif defined( IDENTITY_S_ILI9486 )
#	define IDENTITY_S_VALUE eScreen::ILI9486
#	define IDENTITY_R_VALUE eResolution::R320x480
#elif defined( IDENTITY_S_ILI9488 )
#	define IDENTITY_S_VALUE eScreen::ILI9488
#	define IDENTITY_R_VALUE eResolution::R320x480
#else
#	error "Screen type not yet implemented or not supported"
#endif

/////////////////////////////
// Screen Rendering Library
#if defined( IDENTITY_M_TEENSY_ANY )
#	if defined( IDENTITY_S_ILI9341 )
#		include "Screen/ILI9341_t3_Wrapper.h"
#	elif defined( IDENTITY_S_ILI9488 )
#		include "Screen/ILI9488_t3_Wrapper.h"
#	elif defined( IDENTITY_S_ILI9486 )
#		define USE_GENERIC_ARDUINO_TFT_LIBRARY
#		include "Screen/Arduino_GFX_Wrapper.h"
		using ScreenApi = ArduinoGfx<Arduino_HWSPI, Arduino_ILI9486_18bit>;
#	else
#		error "Screen type is either missing or unsupported for Teensy"
#	endif
#elif defined(IDENTITY_M_SEEEDUINO_XAIO_ANY)
#	define USE_GENERIC_ARDUINO_TFT_LIBRARY
#	include "Screen/Arduino_GFX_Wrapper.h"
#	if defined( IDENTITY_S_ILI9488 )
		using ScreenApi = ArduinoGfx<Arduino_RPiPicoSPI, Arduino_ILI9488_18bit>;
#	else
#		error "Screen type is either missing or unsupported for Seeeduino Xiao"
#	endif
#else
	// If we hit this it probably means this specific microcontroller hasn't been fully implemented yet
#	error "Microcontroller does not yet have any supported screen types"
#endif

static const Identity Identity{ IDENTITY_M_VALUE, IDENTITY_S_VALUE, IDENTITY_R_VALUE };

#endif // __IDENTITY_IMPL_H_
