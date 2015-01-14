using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using TopProcessSvc.Models;

namespace TopProcessSvc.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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