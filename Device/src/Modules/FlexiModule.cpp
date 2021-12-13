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
