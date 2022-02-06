
#include "src/IdentityImplementation.h"

#include "src/PacketType.h"

#include "src/Screen/Screen.h"
#include "src/Screen/PrintUtils.h"
#include "src/Comms.h"
#include "src/Components/Component.h"

#include "src/Modules/FlexiModule.h"

#include <vector>

Screen screen;
Comms comms;

std::vector<Module*> Modules;

unsigned long disconnectedTimestamp = 0;
bool isScreenOn = false;

static const int HIBERNATION_TIMEOUT_MILLISECONDS = 15 * 1000;
char hibernationCountdownBuffer[4] = "";
Printer hibernationCountdownPrinter(&screen);

void PrintHibernateCountdown(unsigned long timeSpentDisconnected)
{
	int timeRemaining = (HIBERNATION_TIMEOUT_MILLISECONDS - timeSpentDisconnected) / 1000;
	snprintf(hibernationCountdownBuffer, 4, "%i", timeRemaining);

	screen.ClearOffset();
	screen.SetTextSize(5);

	Settings settings;
	settings.TextSize = 5;
	settings.Horizontal = HorizontalAlignment::Centre;
	settings.Vertical = VerticalAlignment::Centre;

	hibernationCountdownPrinter.Print(hibernationCountdownBuffer, screen.Width() / 2, screen.Height() / 2, settings);
}

void TurnScreenOn()
{
	// Turn the screen on
	screen.Wakeup();

	// Turn the LED backlight on
	digitalWrite(8, HIGH);

	isScreenOn = true;
}

void TurnScreenOff()
{
	// Turn the backlight off
	digitalWrite(8, LOW);

	// Put the screen into low power sleep mode
	screen.Sleep();

	isScreenOn = false;
}

bool HandleDisconnection()
{
	static bool connected = true;

	if (::Serial)
	{
		// We are connected
		if (!connected)
		{
			// Have have, just this frame, [re]connected
			if (!isScreenOn)
			{
				TurnScreenOn();
				delay(10);
			}

			connected = true;

			Modules.clear();
			screen.FillScreen(COLOUR_BLACK);
		}

		return false;
	}
	else
	{
		// We are disconnected
		if (connected)
		{
			// We have only just disconnected
			connected = false;
			disconnectedTimestamp = millis();

			screen.FillScreen(COLOUR_BLACK);
			screen.ClearOffset();
		}

		if (isScreenOn)
		{
			const unsigned long now = millis();

			const unsigned long timeSpentDisconnected = now - disconnectedTimestamp;
			if (timeSpentDisconnected > HIBERNATION_TIMEOUT_MILLISECONDS)
			{
				TurnScreenOff();
			}
			else
			{
				screen.SetTextSize(1);
				screen.SetCursor(0, 0);
				screen.Print("Companion app not detected...");
				PrintHibernateCountdown(timeSpentDisconnected);
			}
		}

		return true;
	}
}

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
	pinMode(8, OUTPUT);

	screen.Initialize();
	screen.FillScreen(COLOUR_BLACK);

	TurnScreenOn();
}

void loop()
{
	if (HandleDisconnection())
		return;

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
