using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class StillSensorReadEvent : Event
    {
        public int MashTemperatureReading { get; set; }
        public int DistillateTemperatureReading { get; set; }
        public int GasCapacityPercentage { get; set; }

        public StillSensorReadEvent(Guid stillId, int mashTemperatureReading, int distillateTemperatureReading, int gasCapacityPercentage) : base(stillId)
        {
            MashTemperatureReading = mashTemperatureReading;
            DistillateTemperatureReading = distillateTemperatureReading;
            GasCapacityPercentage = gasCapacityPercentage;
        }

        public StillSensorReadEvent()
        {
        }

    }
}
