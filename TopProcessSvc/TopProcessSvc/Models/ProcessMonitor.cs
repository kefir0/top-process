using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TopProcessSvc.Models
{
    public static class ProcessMonitor
    {
        public static IEnumerable<ProcessInfo> GetProcesses()
        {
            return Process.GetProcesses().Select(ProcessToProcessInfo);
        }

        private static ProcessInfo ProcessToProcessInfo(Process process)
        {
            // TODO: Need a timer to monitor CPU usage
            return new ProcessInfo{Id=process.Id, Name = process.ProcessName};
        }
    }
}