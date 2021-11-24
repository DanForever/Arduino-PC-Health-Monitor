#include "Load.h"

void Load::SetValue(Screen* screen, float value)
{
	char buffer[8];
	snprintf(buffer, 8, "%.0f%%", value);

	SetText(screen, buffer);
}
