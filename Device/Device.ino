
#include "src/IdentityImplementation.h"

#include "src/Communications/PacketType.h"

#include "src/Screen/Screen.h"
#include "src/Screen/TimeoutControl.h"

#include "src/Modules/FlexiModule.h"

#include <vector>

Screen screen;

#ifdef MULTITHREADING_ENABLED
#	include "src/Communications/MultiThreaded.h"
	Communications::MultiThreaded comms;
#else
#	include "src/Communications/SingleThreaded.h"
	Communications::SingleThreaded comms;
#endif

TimeoutControl timeoutControl;

std::vector<Module*> Modules;

// Flexi layout
void HandleMessage(Message& message)
{
	ePacketType packetType = (ePacketType)message.Read();

	switch (packetType)
	{
	case ePacketType::VersionRequest:
		comms.SendVersion();
		break;

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
	screen.FillScreen(COLOUR_BLACK);

	timeoutControl.Initialize(&screen);
}

void loop()
{
	timeoutControl.Update(&screen);

	if (!::Serial)
		return;

#ifndef MULTITHREADING_ENABLED
	comms.Update();
#endif

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

#ifdef MULTITHREADING_ENABLED

void setup1()
{
}

void loop1()
{
	if (!::Serial)
		return;

	comms.Update();
}

#endif // MULTITHREADING_ENABLED
