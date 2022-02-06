#include "Module.h"

Module::Module(Screen* screen, Position position)
	: m_screen(screen)
	, m_position(position)
{}

Module::~Module()
{
	for (auto* component : m_components)
	{
		delete component;
	}

	m_components.clear();
}

void Module::Draw()
{
	PushOffset();

	for (auto* component : m_components)
	{
		if (component->HasChanged())
		{
			component->Clear(GetScreen(), COLOUR_BLACK);
			component->Draw(GetScreen());
		}
	}

	PopOffset();
}

void Module::Clear(uint16_t clearColour)
{
	PushOffset();

	for (auto* component : m_components)
	{
		component->Clear(GetScreen(), clearColour);
	}

	PopOffset();
}

void Module::SetNumComponents(uint8_t count)
{
	m_components.reserve(count);
}

void Module::AddComponent(Component* component)
{
	m_components.push_back(component);
}

void Module::PushOffset()
{
	GetScreen()->GetOffset(m_previousOffset.X, m_previousOffset.Y);
	GetScreen()->SetOffset(m_position.X, m_position.Y);
}

void Module::PopOffset()
{
	GetScreen()->SetOffset(m_previousOffset.X, m_previousOffset.Y);
}
