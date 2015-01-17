using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace TopProcessSvc.Models
{
    /// <summary>
    /// Monitors running processes.
    /// </summary>
    public class ProcessMonitor
    {
        /// <summary>
        /// Gets the processes.
        /// </summary>
        public IEnumerable<ProcessInfo> Processes
        {
            get { return _processes.Values; }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            _processes = Process.GetProcesses().Select(ProcessToProcessInfo).ToDictionary(x => x.Id, x => x);
            _lastUpdated = DateTime.Now;
        }


        private double GetCpuUsage(int id, double totalProcessorTime)
        {
            ProcessInfo old;
            if (_processes.TryGetValue(id, out old))
            {
                var timeElapsed = DateTime.Now - _lastUpdated;
                var cpuTime = totalProcessorTime - old.TotalProcessorTime;
                return cpuTime / timeElapsed.TotalMilliseconds / Environment.ProcessorCount;
            }
            return 0;
        }

        private static TimeSpan GetTotalProcessorTime(Process process)
        {
            try
            {
                var totalProcessorTime = process.TotalProcessorTime;
                return totalProcessorTime;
            }
            catch (InvalidOperationException)
            {
                // Possible if process have exited while we retrieve info
                return TimeSpan.Zero;
            }
            catch (Win32Exception)
            {
                // Access denied exception possible for some processes depending on current user account
                return TimeSpan.Zero;
            }
        }

        private ProcessInfo ProcessToProcessInfo(Process process)
        {
            var totalProcessorTime = GetTotalProcessorTime(process).TotalMilliseconds;
            return new ProcessInfo
            {
                Id = process.Id,
                Name = process.ProcessName,
                TotalProcessorTime = totalProcessorTime,
                CpuUsage = GetCpuUsage(process.Id, totalProcessorTime),
                WorkingSet = process.WorkingSet64 / 1024
            };
        }

        private DateTime _lastUpdated = DateTime.Now;
        private Dictionary<int, ProcessInfo> _processes = new Dictionary<int, ProcessInfo>();
    }
}