#ifndef __COMPONENT_FLOAT_WITH_UNIT_H_
#define __COMPONENT_FLOAT_WITH_UNIT_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class FloatWithUnit final : public Component
{
public:
	static const uint8_t MAX_VALUE_LENGTH = 8;
	static const uint8_t MAX_UNIT_LENGTH = 4;

	FloatWithUnit();

	void SetPosition(const Position& position);
	void SetTextSize(uint8_t textSize);
	void SetValue(Screen* screen, float value);
	void SetUnit(Screen* screen, const char* unit);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

private:
	void CalculateTextWidth();

	uint16_t m_valueWidth;
	uint16_t m_unitWidth;

	uint16_t m_textWidth;
	uint16_t m_textHeight;

	Position m_position;
	uint8_t m_textSize;
	uint8_t m_unitTextSize;

	char m_value[MAX_VALUE_LENGTH];
	char m_unit[MAX_UNIT_LENGTH];
};

#endif // __COMPONENT_FLOAT_WITH_UNIT_H_
