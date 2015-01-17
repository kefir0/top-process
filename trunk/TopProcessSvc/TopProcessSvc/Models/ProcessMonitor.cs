using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
            _updateTimer.Elapsed += (sender, args) => Update();
            _updateTimer.Start();
            Update();
        }

        public static ProcessMonitor Instance
        {
            get { return InstanceLazy.Value; }
        }

        /// <summary>
        /// Gets the total physical memory amount, in kilobytes.
        /// </summary>
        public long TotalMemory { get; private set; }

        /// <summary>
        /// Gets the used memory amount, in kilobytes.
        /// </summary>
        public long UsedMemory { get; private set; }

        /// <summary>
        /// Gets the cpu usage, from 0 to 1.
        /// </summary>
        public float CpuUsage { get; private set; }

        /// <summary>
        /// Gets current processes.
        /// </summary>
        public IEnumerable<ProcessInfo> Processes
        {
            get { return _processes.Values; }
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
            };
        }

        private void Update()
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
                    UpdateMemoryUsage();
                    UpdateCpuUsage();
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

        private void UpdateCpuUsage()
        {
            CpuUsage = _cpuCounter.NextValue() / 100;
        }

        private void UpdateMemoryUsage()
        {
            var mgtObj = new ManagementObjectSearcher("Select FreePhysicalMemory, TotalVisibleMemorySize from Win32_OperatingSystem");
            var result = mgtObj.Get().OfType<ManagementObject>().FirstOrDefault();
            if (result != null)
            {
                TotalMemory = Convert.ToInt64(result["TotalVisibleMemorySize"]);
                var free = Convert.ToInt64(result["FreePhysicalMemory"]);
                UsedMemory = TotalMemory - free;
            }
        }

        private static readonly Lazy<ProcessMonitor> InstanceLazy = new Lazy<ProcessMonitor>(() => new ProcessMonitor());

        private readonly PerformanceCounter _cpuCounter = new PerformanceCounter
        {
            CategoryName = "Processor",
            CounterName = "% Processor Time",
            InstanceName = "_Total"
        };

        private readonly object _syncRoot = new object();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Timer _updateTimer;

        private DateTime _lastUpdated = DateTime.Now;
        private Dictionary<int, ProcessInfo> _processes = new Dictionary<int, ProcessInfo>();
    }
}