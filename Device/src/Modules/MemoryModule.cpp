#include "MemoryModule.h"

#include "../Comms.h"

MemoryModule::MemoryModule(Screen* screen, const MemoryConfig& config)
	: Module(screen, config.ScreenPosition)
{
	m_border = new Border(config.Width, config.Height, config.BorderColour);
	m_name = new Text();
	m_usage = new FloatWithUnit();
	m_total = new FloatWithUnit();
	m_icon = new Image();

	m_name->SetPosition(config.NamePosition);
	m_name->SetTextSize(config.NameFontSize);

	m_icon->SetPosition(config.IconPosition);
	m_icon->Centre(config.IconCentred);

	m_usage->SetPosition(config.UsagePosition);
	m_usage->SetTextSize(config.UsageFontSize);
	m_usage->SetUnit(screen, config.UsageUnit);

	m_total->SetPosition(config.TotalPosition);
	m_total->SetTextSize(config.TotalFontSize);
	m_total->SetUnit(screen, config.TotalUnit);

	SetNumComponents(6);
	AddComponent(m_border);
	AddComponent(m_name);
	AddComponent(m_usage);
	AddComponent(m_total);
	AddComponent(m_icon);
}

bool MemoryModule::HandleName(Message& message)
{
	char buffer[Text::MAX_LENGTH];

	message.Read(buffer, Text::MAX_LENGTH);

	m_name->SetText(GetScreen(), buffer);

	return true;
}

bool MemoryModule::HandleFloat(Message& message, FloatWithUnit* component)
{
	float value;
	message.Read(value);

	component->SetValue(GetScreen(), value);

	return true;
}

bool MemoryModule::HandleIcon(Message& message)
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

bool MemoryModule::HandleMessage(Metrics metric, Message& message)
{
	switch (metric)
	{
	case Metrics::MemoryName:
		return HandleName(message);
	case Metrics::MemoryIcon:
		return HandleIcon(message);
	case Metrics::MemoryUsage:
		return HandleFloat(message, m_usage);
	case Metrics::MemoryTotal:
		return HandleFloat(message, m_total);

	default:;
	}

	return false;
}
