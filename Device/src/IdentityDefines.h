#ifndef __IDENTITY_DEFINES_H__
#define __IDENTITY_DEFINES_H__

#include <cstdint>

enum class eMicrocontroller : uint8_t
{
	Teensy32,
	Teensy40,
	SeeediunoXiao,
};

enum class eScreen : uint8_t
{
	ILI9341,
	ILI9486,
	ILI9488,
	NT35510,
};

enum class eResolution : uint8_t
{
	R240x320,
	R320x480,
	R480x800,
};

struct Identity
{
	eMicrocontroller Microcontroller;
	eScreen Screen;
	eResolution Resolution;
};

#endif // __IDENTITY_DEFINES_H__
