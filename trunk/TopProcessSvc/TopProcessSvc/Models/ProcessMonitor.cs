using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace TopProcessSvc.Models
{
    public class ProcessMonitor
    {
        private ProcessMonitor()
        {
            // Use timer for two reasons:
            // - avoid calling Process.GetProcesses on each request for performance
            // - calculate CPU usage in percents
            _updateTimer = new Timer(2000);
            _updateTimer.Elapsed += (sender, args) => UpdateProcesses();
            _updateTimer.Start();
            UpdateProcesses();
        }

        public static ProcessMonitor Instance
        {
            get { return InstanceLazy.Value; }
        }

        public IEnumerable<ProcessInfo> GetProcesses()
        {
            return _processes.Values;
        }

        private double GetCpuUsage(int id, double totalProcessorTime)
        {
            ProcessInfo old;
            if (_processes.TryGetValue(id, out old))
            {
                var timeElapsed = DateTime.Now - old.LastUpdated;
                var cpuTime = totalProcessorTime - old.TotalProcessorTime;
                return cpuTime / timeElapsed.TotalMilliseconds * Environment.ProcessorCount;
            }
            return 0;
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
                LastUpdated = DateTime.Now
            };
        }

        private static TimeSpan GetTotalProcessorTime(Process process)
        {
            try
            {
                var totalProcessorTime = process.TotalProcessorTime;
                return totalProcessorTime;
            }
            catch (Win32Exception)
            {
                // Access denied for system and other user processes
                // TODO: Update deployment documentation to elevate privileges
                return TimeSpan.Zero;
            }
        }

        private void UpdateProcesses()
        {
            // Timer events occur on arbitrary ThreadPool threads
            // No need for thread sync since we only reassign processes dictionary
            // Ideally we should check if previous UpdateProcesses has completed (which can run on another thread) to avoid extra work, 
            // but current timer interval should be more than enough, so keep it simple
            _processes = Process.GetProcesses().Select(ProcessToProcessInfo).ToDictionary(x => x.Id, x => x);
        }

        private static readonly Lazy<ProcessMonitor> InstanceLazy = new Lazy<ProcessMonitor>(() => new ProcessMonitor());

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Timer _updateTimer;

        private Dictionary<int, ProcessInfo> _processes = new Dictionary<int, ProcessInfo>();
    }
}