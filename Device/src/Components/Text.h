#ifndef __COMPONENT_TEXT_H_
#define __COMPONENT_TEXT_H_

#include "Component.h"

//------------------------------------------------------------------------------------------------------
class Text : public Component
{
public:
	static const uint8_t MAX_LENGTH = 32;

	Text();

	void SetPosition(const Position& position);
	void SetTextSize(uint8_t textSize);
	void SetText(Screen* screen, const char* text);

	// Component
	virtual void Draw(Screen* screen) override;
	virtual void Clear(Screen* screen, uint16_t clearColour) override;

	virtual void HandleSetupMessage(Screen* screen, Message& message) override;
	virtual void HandleUpdateMessage(Screen* screen, Message& message) override;

private:
	uint16_t m_textWidth;
	uint16_t m_textHeight;

	Position m_position;
	uint8_t m_textSize;
	uint8_t m_longestLength;
	char m_text[MAX_LENGTH];
};

#endif // __COMPONENT_TEXT_H_
