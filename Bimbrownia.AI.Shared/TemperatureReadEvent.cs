using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class TemperatureReadEvent : Event
    {
        public int TemperatureReading { get; set; }

        public TemperatureReadEvent(Guid stillId, int temperatureReading) : base(stillId)
        {
            TemperatureReading = temperatureReading;
        }

        public TemperatureReadEvent()
        {
        }

    }
}
