#ifndef __COMPONENT_H__
#define __COMPONENT_H__

#include "../Screen.h"
#include "../Metrics.h"

struct Position
{
	int16_t X;
	int16_t Y;
};

//------------------------------------------------------------------------------------------------------
class Component
{
public:
	Component() : m_changed(false) {}

	virtual void Draw(Screen* screen) = 0;
	virtual void Clear(Screen* screen, uint16_t clearColour) = 0;

	bool HasChanged() const { return m_changed; }

protected:
	bool m_changed;
};


#endif // __COMPONENT_H__
