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
