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

#include "TimeoutControl.h"

#include <vector>

void TimeoutControl::Initialize(Screen* screen)
{
	pinMode(PIN_BACKLIGHT, OUTPUT);

	TurnScreenOn(screen);
}

void TimeoutControl::Update(Screen* screen)
{
	if (::Serial)
	{
		// We are m_connected
		if (!m_connected)
		{
			// We have, just this frame, [re]connected
			if (!m_isScreenOn)
			{
				TurnScreenOn(screen);
				delay(10);
			}

			m_connected = true;

			extern std::vector<class Module*> Modules;
			Modules.clear();

			screen->ClearOffset();
			screen->FillScreen(COLOUR_BLACK);
		}
	}
	else
	{
		// We are disconnected
		if (m_connected)
		{
			// We have only just disconnected
			m_connected = false;
			m_disconnectedTimestamp = millis();

			screen->FillScreen(COLOUR_BLACK);
			screen->ClearOffset();
			screen->SetTextSize(1);
			screen->SetCursor(0, 0);
			screen->Print("Companion app not detected...");
		}

		if (m_isScreenOn)
		{
			const unsigned long now = millis();

			const unsigned long timeSpentDisconnected = now - m_disconnectedTimestamp;
			if (timeSpentDisconnected > HIBERNATION_TIMEOUT_MILLISECONDS)
			{
				TurnScreenOff(screen);
			}
			else
			{
				PrintHibernateCountdown(timeSpentDisconnected, screen);
			}
		}
	}
}

void TimeoutControl::PrintHibernateCountdown(unsigned long timeSpentDisconnected, Screen* screen)
{
	int timeRemaining = (HIBERNATION_TIMEOUT_MILLISECONDS - timeSpentDisconnected) / 1000;
	snprintf(m_hibernationCountdownBuffer, HIBERNATION_BUFFER_SIZE, "%i", timeRemaining);

	Settings settings;
	settings.TextSize = 5;
	settings.Horizontal = HorizontalAlignment::Centre;
	settings.Vertical = VerticalAlignment::Centre;

	m_hibernationCountdownPrinter.Print(m_hibernationCountdownBuffer, screen->Width() / 2, screen->Height() / 2, screen, settings);
}

void TimeoutControl::TurnScreenOn(Screen* screen)
{
	// Turn the screen on
	screen->Wakeup();

	// Turn the LED backlight on
	digitalWrite(PIN_BACKLIGHT, HIGH);

	m_isScreenOn = true;
}

void TimeoutControl::TurnScreenOff(Screen* screen)
{
	// Turn the backlight off
	digitalWrite(PIN_BACKLIGHT, LOW);

	// Put the screen into low power sleep mode
	screen->Sleep();

	m_isScreenOn = false;
}
