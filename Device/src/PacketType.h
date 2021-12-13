#ifndef __PACKETTYPE_H__
#define __PACKETTYPE_H__

#include <cstdint>

enum class ePacketType : uint8_t
{
	Version,
	IdentityRequest,
	Identity,
	ModuleDefinition,
	ModuleUpdate,
	Metric,
	Guaranteed,
	GaranteedAck,
	Debug,
};

#endif // __PACKETTYPE_H__
