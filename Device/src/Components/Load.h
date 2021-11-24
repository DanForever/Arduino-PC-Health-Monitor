#ifndef __COMPONENT_LOAD_H_
#define __COMPONENT_LOAD_H_

#include "Text.h"

//------------------------------------------------------------------------------------------------------
class Load final : public Text
{
public:
	void SetValue(Screen* screen, float value);
};

#endif // __COMPONENT_LOAD_H_
