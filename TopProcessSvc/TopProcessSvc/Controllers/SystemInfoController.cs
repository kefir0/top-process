using System;
using System.Web.Http;
using TopProcessSvc.Models;

namespace TopProcessSvc.Controllers
{
    /// <summary>
    /// System information API controller.
    /// </summary>
    public class SystemInfoController : ApiController
    {
        /// <summary>
        ///     GET: /SystemInfo/
        /// </summary>
        public SystemInfo Get()
        {
            var processMonitor = SystemMonitor.Instance;
            return new SystemInfo
            {
                ServerName = Environment.MachineName,
                Processes = processMonitor.Processes,
                MemoryTotal = processMonitor.Memory.TotalMemory,
                MemoryUsed = processMonitor.Memory.UsedMemory,
                CpuUsage = processMonitor.Cpu.CpuUsage
            };
        }
    }
}