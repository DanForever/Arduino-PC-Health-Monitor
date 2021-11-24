#ifndef __COMPONENT_TEMPERATURE_H_
#define __COMPONENT_TEMPERATURE_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Temperature final : public Component
{
public:
	static const uint8_t MAX_LENGTH = 4;

	void SetPosition(const Position& position);
	void SetTextSize(uint8_t textSize);
	void SetValue(Screen* screen, float value);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

private:
	uint16_t m_textWidth;
	uint16_t m_textHeight;

	Position m_position;
	uint8_t m_textSize;
	uint8_t m_unitTextSize;
	char m_text[MAX_LENGTH];
};

#endif // __COMPONENT_TEMPERATURE_H_
