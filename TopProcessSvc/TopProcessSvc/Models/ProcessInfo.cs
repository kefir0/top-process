using System;

namespace TopProcessSvc.Models
{
    public class ProcessInfo
    {
        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the process identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the cpu usage, from 0 to 1.
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the last updated time.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the total processor time, in milliseconds.
        /// </summary>
        public double TotalProcessorTime { get; set; }
    }
}