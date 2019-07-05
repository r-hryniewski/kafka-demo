using System;

namespace Bimbrownia.AI.StatusMonitor
{
    internal class StillStatus
    {
        public Guid Id { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Disabled { get; set; }
        public bool IsRunning => !Disabled.HasValue;
    }
}