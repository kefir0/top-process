using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace TopProcessSvc.Models
{
    public class ProcessMonitor
    {
        private ProcessMonitor()
        {
            // Use timer for two reasons:
            // - avoid calling Process.GetProcesses on each request for performance
            // - calculate CPU usage in percents
            _updateTimer = new Timer(1000); // once a second
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
            catch (Win32Exception)
            {
                // Access denied exception possible for some processes depending on current user account
                // TODO: Update deployment documentation to elevate privileges
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
            };
        }

        private void UpdateProcesses()
        {
            // Timer events occur on arbitrary ThreadPool threads
            // Technically, next timer event can occur before previous update has been finished
            // In that case we don't want to start a new update
            // C# 4.0 style locking with TryEnter http://blogs.msdn.com/b/ericlippert/archive/2009/03/06/locks-and-exceptions-do-not-mix.aspx

            var lockWasTaken = false;
            try
            {
                Monitor.TryEnter(_syncRoot, ref lockWasTaken);
                if (lockWasTaken)
                {
                    _processes = Process.GetProcesses().Select(ProcessToProcessInfo).ToDictionary(x => x.Id, x => x);
                    _lastUpdated = DateTime.Now;
                }
            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(_syncRoot);
                }
            }
        }

        private static readonly Lazy<ProcessMonitor> InstanceLazy = new Lazy<ProcessMonitor>(() => new ProcessMonitor());
        private readonly object _syncRoot = new object();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Timer _updateTimer;

        private DateTime _lastUpdated = DateTime.Now;
        private Dictionary<int, ProcessInfo> _processes = new Dictionary<int, ProcessInfo>();
    }
}