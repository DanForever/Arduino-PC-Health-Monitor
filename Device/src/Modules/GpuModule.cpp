#include "GpuModule.h"

GpuModule::GpuModule(Screen* screen, const GpuConfig& config)
	: Module(screen, config.ScreenPosition)
{
	m_border = new Border(config.Width, config.Height, config.BorderColour);
	m_name = new Text();
	m_icon = new Image();
	m_temperature = new Temperature();
	m_averageCoreClock = new ClockSpeed();
	m_totalLoad = new Load();

	m_name->SetPosition(config.NamePosition);
	m_name->SetTextSize(config.NameFontSize);

	m_icon->SetPosition(config.IconPosition);

	m_temperature->SetPosition(config.TemperaturePosition);
	m_temperature->SetTextSize(config.TemperatureFontSize);

	m_averageCoreClock->SetPosition(config.AverageCoreClockPosition);
	m_averageCoreClock->SetTextSize(config.AverageCoreClockFontSize);

	m_totalLoad->SetPosition(config.TotalLoadPosition);
	m_totalLoad->SetTextSize(config.TotalLoadFontSize);

	SetNumComponents(6);
	AddComponent(m_border);
	AddComponent(m_name);
	AddComponent(m_icon);
	AddComponent(m_temperature);
	AddComponent(m_averageCoreClock);
	AddComponent(m_totalLoad);
}

bool GpuModule::HandleName(Message& message)
{
	char buffer[Text::MAX_LENGTH];

	message.Read(buffer, Text::MAX_LENGTH);

	m_name->SetText(GetScreen(), buffer);

	return true;
}

bool GpuModule::HandleIcon(Message& message)
{
	uint16_t width = -1;
	message.Read(width);

	uint16_t height = -1;
	message.Read(height);

	int32_t position = -1;
	message.Read(position);

	uint16_t buffer[512];
	uint16_t elementsWritten = message.Read(buffer, 512);

	PushOffset();
	m_icon->AddSection(GetScreen(), width, height, position / sizeof(uint16_t), buffer, elementsWritten);
	PopOffset();

	return true;
}

bool GpuModule::HandleAverageClock(Message& message)
{
	float value;
	message.Read(value);

	m_averageCoreClock->SetValue(GetScreen(), value);

	return true;
}

bool GpuModule::HandleTotalLoad(Message& message)
{
	float value;
	message.Read(value);

	m_totalLoad->SetValue(GetScreen(), value);

	return true;
}

bool GpuModule::HandleTemperature(Message& message)
{
	float value;
	message.Read(value);

	m_temperature->SetValue(GetScreen(), value);

	return true;
}

bool GpuModule::HandleMessage(Metrics metric, Message& message)
{
	switch (metric)
	{
	case Metrics::GpuName:
		return HandleName(message);
	case Metrics::GpuIcon:
		return HandleIcon(message);
	case Metrics::GpuClock:
		return HandleAverageClock(message);
	case Metrics::GpuTemperature:
		return HandleTemperature(message);
	case Metrics::GpuLoad:
		return HandleTotalLoad(message);

	default:;
	}

	return false;
}