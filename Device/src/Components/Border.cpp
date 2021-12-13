#include "Border.h"

Border::Border() = default;

void Border::Draw(Screen* screen)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, m_borderColour);

	m_changed = false;
}

void Border::Clear(Screen* screen, uint16_t clearColour)
{
	screen->DrawRoundRect(0, 0, m_borderWidth, m_borderHeight, 8, clearColour);
}

void Border::HandleSetupMessage(Screen* screen, Message& message)
{
	uint16_t dummy;
	message.Read(dummy);
	message.Read(dummy);

	message.Read(m_borderWidth);
	message.Read(m_borderHeight);
	message.Read(m_borderColour);

	m_changed = true;
}

void Border::HandleUpdateMessage(Screen* screen, Message& message)
{
}
