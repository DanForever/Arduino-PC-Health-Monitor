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
	virtual ~Module();

	void Draw();
	void Clear(uint16_t clearColour = COLOUR_BLACK);

	void SetNumComponents(uint8_t count);
	void AddComponent(Component* component);

	void PushOffset();
	void PopOffset();

	virtual void HandleMessage(Message& message) {}

protected:
	Screen* GetScreen();
	Component* GetComponent(uint8_t index);
	uint8_t GetComponentCount() const { return m_components.size(); }

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

inline Component* Module::GetComponent(uint8_t index)
{
	return m_components[index];
}

#endif // __MODULE_H_
