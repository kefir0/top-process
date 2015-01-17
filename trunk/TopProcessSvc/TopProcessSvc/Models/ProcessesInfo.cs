using System.Collections.Generic;

namespace TopProcessSvc.Models
{
    /// <summary>
    /// Server processes DTO.
    /// </summary>
    public class ProcessesInfo
    {
        /// <summary>
        /// Gets or sets the cpu usage, from 0 to 1.
        /// </summary>
        public double CpuUsage { get; set; }

        /// <summary>
        /// Gets or sets the total memory amount, in kilobytes.
        /// </summary>
        public long MemoryTotal { get; set; }

        /// <summary>
        /// Gets or sets the used memory amount, in kilobytes.
        /// </summary>
        public long MemoryUsed { get; set; }

        /// <summary>
        /// Gets or sets the processes.
        /// </summary>
        public IEnumerable<ProcessInfo> Processes { get; set; }

        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string ServerName { get; set; }
    }
}