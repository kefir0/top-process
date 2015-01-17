using System;
using System.Web.Http;
using TopProcessSvc.Models;

namespace TopProcessSvc.Controllers
{
    public class ProcessesController : ApiController
    {
        /// <summary>
        ///     GET: /Processes/
        /// </summary>
        public ProcessesInfo Get()
        {
            var processMonitor = SystemMonitor.Instance;
            return new ProcessesInfo
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