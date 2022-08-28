#include "../IdentityImplementation.h"

#ifdef USE_GENERIC_ARDUINO_TFT_LIBRARY

template<>
ArduinoGfx<Arduino_HWSPI, Arduino_ILI9486_18bit>::ArduinoGfx()
	: m_bus(SCREEN_TFT_DC /* DC */, SCREEN_TFT_CS /* CS */)
	, m_gfx(&m_bus, 19 /* RST */, 0 /* rotation */, false /* IPS */)
{
}

template<>
ArduinoGfx<Arduino_RPiPicoSPI, Arduino_ILI9488_18bit>::ArduinoGfx()
	: m_bus(SCREEN_TFT_DC /* DC */, SCREEN_TFT_CS /* CS */, SCREEN_TFT_SCLK, SCREEN_TFT_MOSI, SCREEN_TFT_MISO)
	, m_gfx(&m_bus, SCREEN_TFT_RST /* RST */, 0 /* rotation */, false /* IPS */)
{
}

#endif
