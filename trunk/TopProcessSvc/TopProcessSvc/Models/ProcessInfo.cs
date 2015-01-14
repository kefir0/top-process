using System;

namespace TopProcessSvc.Models
{
    public class ProcessInfo
    {
        public string Name { get; set; }

        public int Id { get; set; }
        
        public double CpuUsage { get; set; }

        public DateTime LastUpdated { get; set; }
        
        public double TotalProcessorTime { get; set; }
    }
}