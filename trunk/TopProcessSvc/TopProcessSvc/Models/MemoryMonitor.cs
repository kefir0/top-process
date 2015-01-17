using System;
using System.Linq;
using System.Management;

namespace TopProcessSvc.Models
{
    public class MemoryMonitor
    {
        /// <summary>
        /// Gets the total physical memory amount, in kilobytes.
        /// </summary>
        public long TotalMemory { get; private set; }

        /// <summary>
        /// Gets the used memory amount, in kilobytes.
        /// </summary>
        public long UsedMemory { get; private set; }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            var mgtObj = new ManagementObjectSearcher("Select FreePhysicalMemory, TotalVisibleMemorySize from Win32_OperatingSystem");
            var result = mgtObj.Get().OfType<ManagementObject>().FirstOrDefault();
            if (result != null)
            {
                TotalMemory = Convert.ToInt64(result["TotalVisibleMemorySize"]);
                var free = Convert.ToInt64(result["FreePhysicalMemory"]);
                UsedMemory = TotalMemory - free;
            }
        }
    }
}