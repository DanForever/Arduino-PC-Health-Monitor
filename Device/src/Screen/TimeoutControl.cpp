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

TimeoutControl::TimeoutControl(Screen* screen)
	: m_screen(screen)
	, m_hibernationCountdownPrinter(screen)
{
}

void TimeoutControl::Initialize()
{
	pinMode(PIN_BACKLIGHT, OUTPUT);

	TurnScreenOn();
}

void TimeoutControl::Update()
{
	if (::Serial)
	{
		// We are m_connected
		if (!m_connected)
		{
			// Have have, just this frame, [re]connected
			if (!m_isScreenOn)
			{
				TurnScreenOn();
				delay(10);
			}

			m_connected = true;

			extern std::vector<class Module*> Modules;
			Modules.clear();
			m_screen->FillScreen(COLOUR_BLACK);
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

			m_screen->FillScreen(COLOUR_BLACK);
			m_screen->ClearOffset();
		}

		if (m_isScreenOn)
		{
			const unsigned long now = millis();

			const unsigned long timeSpentDisconnected = now - m_disconnectedTimestamp;
			if (timeSpentDisconnected > HIBERNATION_TIMEOUT_MILLISECONDS)
			{
				TurnScreenOff();
			}
			else
			{
				m_screen->SetTextSize(1);
				m_screen->SetCursor(0, 0);
				m_screen->Print("Companion app not detected...");
				PrintHibernateCountdown(timeSpentDisconnected);
			}
		}
	}
}

void TimeoutControl::PrintHibernateCountdown(unsigned long timeSpentDisconnected)
{
	int timeRemaining = (HIBERNATION_TIMEOUT_MILLISECONDS - timeSpentDisconnected) / 1000;
	snprintf(m_hibernationCountdownBuffer, 4, "%i", timeRemaining);

	m_screen->ClearOffset();
	m_screen->SetTextSize(5);

	Settings settings;
	settings.TextSize = 5;
	settings.Horizontal = HorizontalAlignment::Centre;
	settings.Vertical = VerticalAlignment::Centre;

	m_hibernationCountdownPrinter.Print(m_hibernationCountdownBuffer, m_screen->Width() / 2, m_screen->Height() / 2, settings);
}

void TimeoutControl::TurnScreenOn()
{
	// Turn the screen on
	m_screen->Wakeup();

	// Turn the LED backlight on
	digitalWrite(PIN_BACKLIGHT, HIGH);

	m_isScreenOn = true;
}

void TimeoutControl::TurnScreenOff()
{
	// Turn the backlight off
	digitalWrite(PIN_BACKLIGHT, LOW);

	// Put the screen into low power sleep mode
	m_screen->Sleep();

	m_isScreenOn = false;
}
