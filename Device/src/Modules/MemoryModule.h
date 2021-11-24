#ifndef __MODULE_MEMORY_H_
#define __MODULE_MEMORY_H_

#include "Module.h"

#include "../Components/Border.h"
#include "../Components/Text.h"
#include "../Components/Image.h"
#include "../Components/Temperature.h"
#include "../Components/ClockSpeed.h"
#include "../Components/Load.h"
#include "../Components/FloatWithUnit.h"

//------------------------------------------------------------------------------------------------------
struct MemoryConfig
{
	Position ScreenPosition;

	int16_t Width;
	int16_t Height;

	uint16_t BorderColour;

	Position NamePosition;
	uint8_t NameFontSize;

	Position IconPosition;
	bool IconCentred;

	Position UsagePosition;
	uint8_t UsageFontSize;
	const char* UsageUnit;

	Position TotalPosition;
	uint8_t TotalFontSize;
	const char* TotalUnit;
};

//------------------------------------------------------------------------------------------------------
class MemoryModule : public Module
{
public:
	MemoryModule(Screen* screen, const MemoryConfig& config);

	// Module
	virtual bool HandleMessage(Metrics metric, Message& message) override;

private:
	bool HandleName(Message& message);
	bool HandleFloat(Message& message, FloatWithUnit* component);
	bool HandleIcon(Message& message);

	Border* m_border;
	Text* m_name;
	FloatWithUnit* m_usage;
	FloatWithUnit* m_total;
	Image* m_icon;
};

#endif // __MODULE_MEMORY_H_
