#ifndef __COMPONENT_IMAGE_H_
#define __COMPONENT_IMAGE_H_

#include "Component.h"

class Module;

//------------------------------------------------------------------------------------------------------
class Image final : public Component
{
public:
	Image() = default; // TEMP

	Image(Module* parent);

	void Centre(bool centre);
	void SetPosition(const Position& position);
	void AddSection(Screen* screen, uint16_t width, uint16_t height, int32_t position, const uint16_t* data, uint16_t dataLength);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour);

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	Position m_position;
	Module* m_parent;
	bool m_centre;
};

#endif // __COMPONENT_IMAGE_H_
