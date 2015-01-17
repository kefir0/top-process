using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TopProcessSvc.Models
{
    public class NotificationDispatcher
    {
        public NotificationDispatcher(SystemMonitor systemMonitor)
        {
            if (systemMonitor == null)
            {
                throw new ArgumentNullException("systemMonitor");
            }

            _systemMonitor = systemMonitor;

            // Defaults
            CpuUsageLimit = 0;// 0.8;
            MemoryUsageLimit = 0;// 0.8;
        }

        /// <summary>
        /// Gets or sets the cpu usage limit, from 0 to 1 (0 to 100%).
        /// </summary>
        public double CpuUsageLimit { get; set; }

        /// <summary>
        /// Gets or sets the memory usage limit, from 0 to 1 (0 to 100%).
        /// </summary>
        public double MemoryUsageLimit { get; set; }

        /// <summary>
        /// Analyzes system performance and sends notifications to connected clients when necessary (CPU, memory and other limits exceeded).
        /// </summary>
        public void Update()
        {
            var messages = new[] {GetCpuMessage(), GetMemoryMessage()}.Where(x => x != null).ToList();
            if (messages.Any())
            {
                _notificationClients.All.broadcastMessage(messages.Aggregate((acc, val) => string.Format("{0}\n{1}", acc, val)));
            }
        }

        private string GetCpuMessage()
        {
            var cpuUsage = _systemMonitor.Cpu.CpuUsage;
            if (cpuUsage > CpuUsageLimit)
            {
                return string.Format("CPU usage exceeds limit. {0}% usage exceeds {1}% limit.", ToPercent(cpuUsage), ToPercent(CpuUsageLimit));
            }

            return null;
        }

        private string GetMemoryMessage()
        {
            var usedMemory = _systemMonitor.Memory.UsedMemory;
            var totalMemory = _systemMonitor.Memory.TotalMemory;
            var memoryUsage = (double) usedMemory / totalMemory;
            if (memoryUsage > MemoryUsageLimit)
            {
                return string.Format("Memory usage exceeds limit. {0} KB used of {1} KB total. {2}% usage exceeds {3}% limit.", usedMemory, totalMemory,
                    ToPercent(memoryUsage), ToPercent(MemoryUsageLimit));
            }

            return null;
        }

        private static int ToPercent(double val)
        {
            return (int) (val * 100);
        }

        private readonly IHubConnectionContext<dynamic> _notificationClients = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients;
        private readonly SystemMonitor _systemMonitor;
    }
}