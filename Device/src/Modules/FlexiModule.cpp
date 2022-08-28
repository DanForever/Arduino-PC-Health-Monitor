// Arduino PC Health Monitor (Device firmware)
// Polls the hardware sensors for data and forwards them on to the arduino device
// Copyright (C) 2022 Daniel Neve
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#include "FlexiModule.h"

#include "../Components/Border.h"
#include "../Components/Text.h"
#include "../Components/FloatWithUnit.h"
#include "../Components/Image.h"

namespace
{
	enum class MessageType
	{
		ComponentDefinition,
		ComponentUpdate
	};

	enum class ComponentType
	{
		Border,
		Text,
		FloatWithUnit,
		Icon,
	};
}

FlexiModule::FlexiModule(Screen* screen, Position position)
	: Module(screen, position)
{
}

FlexiModule::~FlexiModule() = default;

void FlexiModule::HandleMessage(Message& message)
{
	MessageType messageType = (MessageType)message.Read();

	switch (messageType)
	{
	case MessageType::ComponentDefinition:
		HandleComponentDefinitionMessage(message);
		break;

	case MessageType::ComponentUpdate:
		HandleComponentUpdateMessage(message);
		break;
	}
}

void FlexiModule::HandleComponentDefinitionMessage(Message& message)
{
	ComponentType componentType = (ComponentType)message.Read();

	Component* component = nullptr;
	switch (componentType)
	{
	case ComponentType::Border:
		component = new Border();
		break;

	case ComponentType::Text:
		component = new Text();
		break;

	case ComponentType::FloatWithUnit:
		component = new FloatWithUnit();
		break;

	case ComponentType::Icon:
		component = new Image(this);
		break;
	}

	if (!component)
		return;

	component->HandleSetupMessage(GetScreen(), message);

	AddComponent(component);
}

void FlexiModule::HandleComponentUpdateMessage(Message& message)
{
	uint8_t componentIndex;

	message.Read(componentIndex);

	Component* component = GetComponent(componentIndex);
	component->HandleUpdateMessage(GetScreen(), message);
}
