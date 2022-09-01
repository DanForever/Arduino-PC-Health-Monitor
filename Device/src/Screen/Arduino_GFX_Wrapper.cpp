// Arduino PC Health Monitor (Device firmware)
// Polls the hardware sensors for data and forwards them on to the arduino device
// Copyright (C) 2022 Daniel Neve
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#include "../IdentityImplementation.h"

#ifdef USE_GENERIC_ARDUINO_TFT_LIBRARY

template<>
ArduinoGfx<Arduino_HWSPI, Arduino_ILI9486_18bit>::ArduinoGfx()
	: m_bus(SCREEN_TFT_DC /* DC */, SCREEN_TFT_CS /* CS */)
	, m_gfx(&m_bus, SCREEN_TFT_RST /* RST */, 0 /* rotation */, false /* IPS */)
{
}

#ifdef IDENTITY_M_SEEEDUINO_XAIO_RP2040

template<>
ArduinoGfx<Arduino_RPiPicoSPI, Arduino_ILI9488_18bit>::ArduinoGfx()
	: m_bus(SCREEN_TFT_DC /* DC */, SCREEN_TFT_CS /* CS */, SCREEN_TFT_SCLK, SCREEN_TFT_MOSI, SCREEN_TFT_MISO)
	, m_gfx(&m_bus, SCREEN_TFT_RST /* RST */, 0 /* rotation */, false /* IPS */)
{
}

#endif // IDENTITY_M_SEEEDUINO_XAIO_RP2040

#endif
