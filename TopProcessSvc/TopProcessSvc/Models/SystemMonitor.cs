﻿using System;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace TopProcessSvc.Models
{
    /// <summary>
    /// Monitors multiple aspects of system performance.
    /// Updates itself once a second.
    /// </summary>
    public class SystemMonitor
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static SystemMonitor Instance
        {
            get { return InstanceLazy.Value; }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemMonitor"/> class from being created.
        /// </summary>
        private SystemMonitor()
        {
            // Use timer for two reasons:
            // - avoid calling Process.GetProcesses on each request for performance
            // - calculate CPU usage in percents
            _updateTimer = new Timer(1000); // once a second
            _updateTimer.Elapsed += (sender, args) => Update();
            _updateTimer.Start();

            _notifications = new NotificationDispatcher(this);

            Update();
        }

        /// <summary>
        /// Gets the cpu info.
        /// </summary>
        public CpuMonitor Cpu
        {
            get { return _cpu; }
        }

        /// <summary>
        /// Gets the memory info.
        /// </summary>
        public MemoryMonitor Memory
        {
            get { return _memory; }
        }

        /// <summary>
        /// Gets the processes.
        /// </summary>
        public IEnumerable<ProcessInfo> Processes
        {
            get { return _processes.Processes; }
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
                    _cpu.Update();
                    _memory.Update();
                    _processes.Update();
                    _notifications.Update();
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

        private static readonly Lazy<SystemMonitor> InstanceLazy = new Lazy<SystemMonitor>(() => new SystemMonitor());

        private readonly CpuMonitor _cpu = new CpuMonitor();
        private readonly MemoryMonitor _memory = new MemoryMonitor();
        private readonly ProcessMonitor _processes = new ProcessMonitor();
        private readonly NotificationDispatcher _notifications;
        private readonly object _syncRoot = new object();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Timer _updateTimer;
    }
}