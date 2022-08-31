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

#ifndef __BACKLIGHT_CONTROL_H__
#define __BACKLIGHT_CONTROL_H__

#include "PrintUtils.h"

class TimeoutControl
{
public:
	void Initialize(Screen* screen);
	void Update(Screen* screen);

private:
	void PrintHibernateCountdown(unsigned long timeSpentDisconnected, Screen* screen);

	void TurnScreenOn(Screen* screen);
	void TurnScreenOff(Screen* screen);

	static const int HIBERNATION_TIMEOUT_MILLISECONDS = 15 * 1000;

	Screen* m_screen;
	unsigned long m_disconnectedTimestamp = 0;
	bool m_isScreenOn = false;
	bool m_connected = true;

	char m_hibernationCountdownBuffer[4] = "";
	Printer m_hibernationCountdownPrinter;
};

#endif // __BACKLIGHT_CONTROL_H__