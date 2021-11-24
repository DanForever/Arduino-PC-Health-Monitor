
#include "src/Screen.h"
#include "src/Comms.h"
#include "src/images/cpu/common.h"
#include "src/images/cpu/amd.h"
#include "src/images/cpu/intel.h"
#include "src/images/gpu/geforce.h"
#include "src/Components/Component.h"

#include "src/Modules/CpuModule.h"
#include "src/Modules/GpuModule.h"
#include "src/Modules/MemoryModule.h"

#include <vector>
#include <cstring>

Screen screen;
Comms comms;

enum class MessageCategory
{
	Metric,
	Guaranteed,
	GaranteedAck,
	Debug,
};

enum ModuleTypes
{
	Cpu,
	Gpu,
	Memory,

	Count
};

static const uint8_t NumModules = 2;
Module* Modules[ModuleTypes::Count];

void SensorUpdate(Message& message)
{
	Metrics metric = (Metrics)message.Read();

	for (uint8_t i = 0; i < ModuleTypes::Count; ++i)
	{
		if (Modules[i]->HandleMessage(metric, message))
			break;
	}
}

void HandleMessage(Message& message)
{
	MessageCategory messageCategory = (MessageCategory)message.Read();

	switch (messageCategory)
	{
	case MessageCategory::Metric:
		SensorUpdate(message);
		break;

	case MessageCategory::Guaranteed:
		{
			uint16_t packetId;
			message.Read(packetId);
			HandleMessage(message);

			comms.Ack(packetId);
		}
		break;

	case MessageCategory::Debug:
		Serial.println("Debug message recieved!");
		break;

	default:;
		Serial.print("Unrecognised message category: ");
		Serial.println((int)messageCategory);
	}
}

void setup()
{
	screen.Initialize();

	screen.FillScreen(ILI9341_BLACK);

	CpuConfig cpuConfig =
	{
		{ 0, 0 },

		240,
		120,

		ILI9341_BLUE,

		{ 16, 8 },
		1,

		{ 16 + 44, 25 + 44 },
		true,

		{ 105, 25 },
		3,

		{ 105, 55 },
		3,

		{ 105, 85 },
		3
	};

	Modules[ModuleTypes::Cpu] = new CpuModule(&screen, cpuConfig);

	GpuConfig gpuConfig =
	{
		{ 0, 121 },

		240,
		120,

		ILI9341_GREEN,

		{ 16, 8 },
		1,

		{ 16, 25 },

		{ 105, 25 },
		3,

		{ 105, 55 },
		3,

		{ 105, 85 },
		3
	};

	Modules[ModuleTypes::Gpu] = new GpuModule(&screen, gpuConfig);

	MemoryConfig memConfig =
	{
		{ 0, 120 + 120 + 1 },

		240,
		320 - 241,

		ILI9341_YELLOW,

		{ 16, 8 },
		1,

		{ 16 + 44, 25 + 23 },
		true,

		{ 105, 25 },
		3,
		"GB",


		{ 105 + 60, 25 },
		3,
		"GB",

	};

	Modules[ModuleTypes::Memory] = new MemoryModule(&screen, memConfig);
}

void loop(void)
{
	comms.Update();

	if (comms.MessageReady())
	{
		HandleMessage(comms.GetMessage());
		comms.ClearMessage();
	}

	for (uint8_t i = 0; i < ModuleTypes::Count; ++i)
	{
		Modules[i]->Draw();
	}
}
