using System.Collections.Generic;
using System.Web.Http;
using TopProcessSvc.Models;

namespace TopProcessSvc.Controllers
{
    public class ProcessesController : ApiController
    {
        //
        // GET: /Processes/

        public IEnumerable<Process> Get()
        {
            yield return new Process {Name = "system", CpuUsage = 13, Id = 1};
            yield return new Process {Name = "svchost.exe", CpuUsage = 23, Id = 20};
        }
    }
}