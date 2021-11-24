#ifndef __MODULE_H_
#define __MODULE_H_

#include "../Components/Component.h"
#include "../Comms.h"

#include <vector>

//------------------------------------------------------------------------------------------------------
class Module
{
public:
	Module(Screen* screen, Position position);

	void Draw();
	void Clear(uint16_t clearColour = ILI9341_BLACK);

	void SetNumComponents(uint8_t count);
	void AddComponent(Component* component);

	void PushOffset();
	void PopOffset();

	Screen* GetScreen();

	virtual bool HandleMessage(Metrics metric, Message& message) = 0;

private:
	Screen* m_screen;
	Position m_position;
	Position m_previousOffset;

	std::vector<Component*> m_components;
};

inline Screen* Module::GetScreen()
{
	return m_screen;
}

#endif // __MODULE_H_
