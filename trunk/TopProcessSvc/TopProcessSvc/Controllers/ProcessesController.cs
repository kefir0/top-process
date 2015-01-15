using System;
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
            return new ProcessesInfo
            {
                ServerName = Environment.MachineName,
                Processes = ProcessMonitor.Instance.GetProcesses()
            };
        }
    }
}