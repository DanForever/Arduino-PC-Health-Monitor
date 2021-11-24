#ifndef __MODULE_GPU_H_
#define __MODULE_GPU_H_

#include "Module.h"

#include "../Components/Border.h"
#include "../Components/Text.h"
#include "../Components/Image.h"
#include "../Components/Temperature.h"
#include "../Components/ClockSpeed.h"
#include "../Components/Load.h"

//------------------------------------------------------------------------------------------------------
struct GpuConfig
{
	Position ScreenPosition;

	int16_t Width;
	int16_t Height;

	uint16_t BorderColour;

	Position NamePosition;
	uint8_t NameFontSize;

	Position IconPosition;

	Position TemperaturePosition;
	uint8_t TemperatureFontSize;

	Position AverageCoreClockPosition;
	uint8_t AverageCoreClockFontSize;

	Position TotalLoadPosition;
	uint8_t TotalLoadFontSize;
};

//------------------------------------------------------------------------------------------------------
class GpuModule : public Module
{
public:
	GpuModule(Screen* screen, const GpuConfig& config);

	// Module
	virtual bool HandleMessage(Metrics metric, Message& message) override;

private:
	bool HandleName(Message& message);
	bool HandleIcon(Message& message);
	bool HandleAverageClock(Message& message);
	bool HandleTemperature(Message& message);
	bool HandleTotalLoad(Message& message);

	Border* m_border;
	Text* m_name;
	Image* m_icon;
	Temperature* m_temperature;
	ClockSpeed* m_averageCoreClock;
	Load* m_totalLoad;
};

#endif // __MODULE_GPU_H_
