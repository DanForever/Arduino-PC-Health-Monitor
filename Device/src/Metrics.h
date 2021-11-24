#ifndef __METRICS_H__
#define __METRICS_H__

enum class Metrics
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
