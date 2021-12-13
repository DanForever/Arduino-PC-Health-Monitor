#ifndef __COMPONENT_BORDER_H_
#define __COMPONENT_BORDER_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Border final : public Component
{
public:
	Border();

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	int16_t m_borderWidth;
	int16_t m_borderHeight;
	uint16_t m_borderColour;
};

#endif // __COMPONENT_BORDER_H_
