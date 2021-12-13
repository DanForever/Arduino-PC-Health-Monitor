
#include "src/IdentityImplementation.h"

#include "src/PacketType.h"

#include "src/Screen.h"
#include "src/Comms.h"
#include "src/Components/Component.h"

#include "src/Modules/FlexiModule.h"

#include <vector>

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
