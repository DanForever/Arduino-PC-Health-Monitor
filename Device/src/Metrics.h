#ifndef __METRICS_H__
#define __METRICS_H__

#include <cstdint>

enum class Metrics : uint8_t
{
    CpuName,
    CpuIcon,
	CpuAverageClock,
    CpuTemperature,
    CpuTotalLoad,

    GpuName,
    GpuIcon,
    GpuClock,
    GpuTemperature,
    GpuLoad,

    MemoryName,
    MemoryIcon,
    MemoryTotal,
    MemoryUsage,
};

#endif // __METRICS_H__
