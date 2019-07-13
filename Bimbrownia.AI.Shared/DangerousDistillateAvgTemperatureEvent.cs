using System;

namespace Bimbrownia.AI.Shared
{
    public class DangerousDistillateAvgTemperatureEvent : Event
    {
        public double AverageDistillateTemperature { get; set; }

        public DangerousDistillateAvgTemperatureEvent(Guid stillId, double averageDistillateTemperature) : base(stillId)
        {
            this.AverageDistillateTemperature = averageDistillateTemperature;
        }
        public DangerousDistillateAvgTemperatureEvent()
        {

        }
    }
}