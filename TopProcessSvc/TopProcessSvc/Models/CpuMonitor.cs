using System.Diagnostics;

namespace TopProcessSvc.Models
{
    public class CpuMonitor
    {
        /// <summary>
        /// Gets the cpu usage, from 0 to 1.
        /// </summary>
        public float CpuUsage { get; private set; }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            CpuUsage = _cpuCounter.NextValue() / 100;
        }

        private readonly PerformanceCounter _cpuCounter = new PerformanceCounter
        {
            CategoryName = "Processor",
            CounterName = "% Processor Time",
            InstanceName = "_Total"
        };
    }
}