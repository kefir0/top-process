using System.Collections.Generic;
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
        public IEnumerable<ProcessInfo> Get()
        {
            return ProcessMonitor.Instance.GetProcesses();
        }
    }
}