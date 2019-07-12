using Bimbrownia.AI.Shared;
using System;
using System.Linq;

namespace Bimbrownia.AI.SensorMonitor
{
    internal class StillSensorData
    {
        internal Guid StillId { get; set; }

        private readonly CircullarBuffer<StillSensorReading> readings = new CircullarBuffer<StillSensorReading>(10);

        internal double AverageMashTemperatureReading => readings.Select(r => r.MashTemperatureReading).Average();
        internal double AverageDistillateTemperatureReading => readings.Select(r => r.DistillateTemperatureReading).Average();
        internal double LastGasCapacityPercentage => readings.Select(r => r.GasCapacityPercentage).LastOrDefault();

        public bool HasRealiableReadings => readings.IsFull;

        public bool Disabled { get; private set; }

        internal StillSensorData(Guid stillId)
        {
            StillId = stillId;
        }

        internal void ProcessSensorReading(int mashTemperatureReading, int distillateTemperatureReading, int gasCapacityPercentage)
        {
            readings.Add(new StillSensorReading(mashTemperatureReading, distillateTemperatureReading, gasCapacityPercentage));
        }

        internal void Disable()
        {
            Disabled = true;
        }
    }
    internal class StillSensorReading
    {
        internal int MashTemperatureReading { get; private set; }
        internal int DistillateTemperatureReading { get; private set; }
        internal int GasCapacityPercentage { get; private set; }

        internal StillSensorReading(int mashTemperatureReading, int distillateTemperatureReading, int gasCapacityPercentage)
        {
            MashTemperatureReading = mashTemperatureReading;
            DistillateTemperatureReading = distillateTemperatureReading;
            GasCapacityPercentage = gasCapacityPercentage;
        }
    }
}