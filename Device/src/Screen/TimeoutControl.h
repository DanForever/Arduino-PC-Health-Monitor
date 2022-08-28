#ifndef __BACKLIGHT_CONTROL_H__
#define __BACKLIGHT_CONTROL_H__

#include "PrintUtils.h"

class TimeoutControl
{
public:
	TimeoutControl(Screen* screen);
	void Initialize();
	void Update();

private:
	void PrintHibernateCountdown(unsigned long timeSpentDisconnected);

	void TurnScreenOn();
	void TurnScreenOff();

	static const int HIBERNATION_TIMEOUT_MILLISECONDS = 15 * 1000;

	Screen* m_screen;
	unsigned long m_disconnectedTimestamp = 0;
	bool m_isScreenOn = false;
	bool m_connected = true;

	char m_hibernationCountdownBuffer[4] = "";
	Printer m_hibernationCountdownPrinter;
};

#endif // __BACKLIGHT_CONTROL_H__