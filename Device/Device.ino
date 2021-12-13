
#include "src/IdentityImplementation.h"

#include "src/PacketType.h"

#include "src/Screen.h"
#include "src/Comms.h"
#include "src/Components/Component.h"

#include "src/Modules/CpuModule.h"
#include "src/Modules/GpuModule.h"
#include "src/Modules/MemoryModule.h"
#include "src/Modules/FlexiModule.h"

#include <vector>
#include <cstring>

Screen screen;
Comms comms;

std::vector<Module*> Modules;

// Flexi layout
void HandleMessage(Message& message)
{
	ePacketType packetType = (ePacketType)message.Read();

	switch (packetType)
	{
	case ePacketType::ModuleDefinition:
		HandleModuleDefinition(message);
		break;

	case ePacketType::ModuleUpdate:
		HandleModuleUpdate(message);
		break;

	case ePacketType::IdentityRequest:
		comms.SendIdentity();
		break;

	case ePacketType::Guaranteed:
	{
		uint16_t packetId;
		message.Read(packetId);
		HandleMessage(message);

		comms.Ack(packetId);
	}
	break;

	case ePacketType::Debug:
		Serial.println("Debug message recieved!");
		break;

	default:;
		Serial.print("Unrecognised message category: ");
		Serial.println((int)packetType);
	}
}

void CreateModule(Message& message)
{
	Position position;
	message.Read(position.X);
	message.Read(position.Y);

	uint8_t componentCount;
	message.Read(componentCount);

	FlexiModule* module = new FlexiModule(&screen, position);
	module->SetNumComponents(componentCount);
	Modules.push_back(module);

	for (uint8_t i = 0; i < componentCount; ++i)
	{
		module->HandleMessage(message);
	}
}

void HandleModuleDefinition(Message& message)
{
	uint8_t moduleCount = message.Read();

	for (uint8_t i = 0; i < moduleCount; ++i)
	{
		CreateModule(message);
	}
}

void HandleModuleUpdate(Message& message)
{
	uint8_t moduleIndex = message.Read();

	Modules[moduleIndex]->HandleMessage(message);
}

void setup()
{
	screen.Initialize();
	screen.FillScreen(ILI9341_BLACK);
}

void loop()
{
	comms.Update();

	if (comms.MessageReady())
	{
		HandleMessage(comms.GetMessage());
		comms.ClearMessage();
	}

	for (uint8_t i = 0; i < Modules.size(); ++i)
	{
		Modules[i]->Draw();
	}
}


// Fixed layout
/*
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
	ePacketType packetType = (ePacketType)message.Read();

	switch (packetType)
	{
	case ePacketType::Metric:
		SensorUpdate(message);
		break;

	case ePacketType::IdentityRequest:
		comms.SendIdentity();
		break;

	case ePacketType::Guaranteed:
		{
			uint16_t packetId;
			message.Read(packetId);
			HandleMessage(message);

			comms.Ack(packetId);
		}
		break;

	case ePacketType::Debug:
		Serial.println("Debug message recieved!");
		break;

	default:;
		Serial.print("Unrecognised message category: ");
		Serial.println((int)packetType);
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
//*/