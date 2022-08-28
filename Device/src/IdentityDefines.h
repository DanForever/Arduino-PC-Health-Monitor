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

#ifndef __IDENTITY_DEFINES_H__
#define __IDENTITY_DEFINES_H__

#include <cstdint>

enum class eMicrocontroller : uint8_t
{
	Teensy32,
	Teensy40,
	SeeediunoXiao,
	SeeediunoXiaoRp2040,
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
