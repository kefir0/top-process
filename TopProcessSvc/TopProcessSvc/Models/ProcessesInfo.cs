using System.Collections.Generic;

namespace TopProcessSvc.Models
{
    /// <summary>
    /// Server processes DTO.
    /// </summary>
    public class ProcessesInfo
    {
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the processes.
        /// </summary>
        public IEnumerable<ProcessInfo> Processes { get; set; }
    }
}