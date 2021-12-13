#ifndef __FLEXIMODULE_H__
#define __FLEXIMODULE_H__

#include "Module.h"

//------------------------------------------------------------------------------------------------------
class FlexiModule : public Module
{
public:
	FlexiModule(Screen* screen, Position position);
	virtual ~FlexiModule() override;

	// Module
	virtual void HandleMessage(Message& message) override;

private:
	void HandleComponentDefinitionMessage(Message& message);
	void HandleComponentUpdateMessage(Message& message);
};


#endif // __FLEXIMODULE_H__
