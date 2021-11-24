#ifndef __COMPONENT_BORDER_H_
#define __COMPONENT_BORDER_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Border final : public Component
{
public:
	Border(int16_t width, int16_t height, uint16_t colour);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour);

private:
	int16_t m_borderWidth;
	int16_t m_borderHeight;
	uint16_t m_borderColour;
};

#endif // __COMPONENT_BORDER_H_
