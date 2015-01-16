﻿using System;
using System.Web.Http;
using System.Web.Http.Cors;
using TopProcessSvc.Models;

namespace TopProcessSvc.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProcessesController : ApiController
    {
        /// <summary>
        ///     GET: /Processes/
        /// </summary>
        public ProcessesInfo Get()
        {
            var processMonitor = ProcessMonitor.Instance;
            return new ProcessesInfo
            {
                ServerName = Environment.MachineName,
                Processes = processMonitor.Processes,
                MemoryTotal = processMonitor.TotalMemory,
                MemoryUsed = processMonitor.UsedMemory,
                CpuUsage = processMonitor.CpuUsage
            };
        }
    }
}