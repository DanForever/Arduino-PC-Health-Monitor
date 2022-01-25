#ifndef __IDENTITY_DEVICE_H__
#define __IDENTITY_DEVICE_H__

//#define IDENTITY_M_TEENSY32
//#define IDENTITY_M_TEENSY40
//#define IDENTITY_M_SEEEDUINOXAIO

//#define IDENTITY_S_ILI9341
//#define IDENTITY_S_ILI9486
//#define IDENTITY_S_ILI9488
//#define IDENTITY_S_NT35510

#if defined( IDENTITY_S_ILI9341 )
#	define IDENTITY_R_240x320
#elif defined( IDENTITY_S_ILI9488 )
#	define IDENTITY_R_320x480
#else
#	error Could not figure out resolution from screen driver
#endif

//#define IDENTITY_R_240x320
//#define IDENTITY_R_320x480
//#define IDENTITY_R_480x800

#endif // __IDENTITY_DEVICE_H__
