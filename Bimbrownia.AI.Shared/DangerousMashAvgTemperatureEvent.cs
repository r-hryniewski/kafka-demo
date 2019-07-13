using System;

namespace Bimbrownia.AI.Shared
{
    public class DangerousMashAvgTemperatureEvent : Event
    {
        public double AverageMashTemperature { get; set; }

        public DangerousMashAvgTemperatureEvent(Guid stillId, double averageDistillateTemperature) : base(stillId)
        {
            this.AverageMashTemperature = averageDistillateTemperature;
        }
        public DangerousMashAvgTemperatureEvent()
        {

        }
    }
}