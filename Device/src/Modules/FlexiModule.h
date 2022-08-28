/*
 * Arduino PC Health Monitor (Device firmware)
 * Polls the hardware sensors for data and forwards them on to the arduino device
 * Copyright (C) 2022 Daniel Neve
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

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
