using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.SensorMonitor
{
    class Program
    {
        public static readonly ConsumerConfig ConsumerConfig = new ConsumerConfig
        {
            GroupId = nameof(Bimbrownia.AI.SensorMonitor),
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        private static readonly ConcurrentDictionary<Guid, StillSensorData> sensorDataDictionary = new ConcurrentDictionary<Guid, StillSensorData>();

        static async Task Main(string[] args)
        {
            Console.Title = "Sensor Monitor";

            Task.Run(MonitorStillSensorRead);
            Task.Run(MonitorStillDisabled);

            while (true)
            {
                Console.Clear();
                ReadSensorData();
                await Task.Delay(1000);
            }
        }

        private static void MonitorStillSensorRead()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(Constants.TopicName_StillSensorRead);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);

                            var ev = JsonConvert.DeserializeObject<StillSensorReadEvent>(cr.Value);
                            HandleStillSensorReadEvent(ev);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }
        }

        private static async Task MonitorStillDisabled()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(Constants.TopicName_StillDisabled);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);

                            var ev = JsonConvert.DeserializeObject<StillDisabledEvent>(cr.Value);
                            HandleStillDisabledEvent(ev);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    c.Close();
                }
            }
        }

        private static void HandleStillDisabledEvent(StillDisabledEvent ev)
        {
            if (sensorDataDictionary.TryGetValue(ev.StillId, out var item))
            {
                item.Disable();
            }
        }

        private static void HandleStillSensorReadEvent(StillSensorReadEvent ev)
        {
            sensorDataDictionary.TryAdd(ev.StillId, new StillSensorData(ev.StillId));

            if(sensorDataDictionary.TryGetValue(ev.StillId, out var item) && !item.Disabled)
            {
                item.ProcessSensorReading(ev.MashTemperatureReading, ev.DistillateTemperatureReading, ev.GasCapacityPercentage);
            }
        }

        private static void ReadSensorData()
        {
            Console.WriteLine($"----------BIMBROWNIA AI----------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
            Console.WriteLine("Sensor readings:");

            if (sensorDataDictionary.IsEmpty)
            {
                Console.WriteLine("No stills running");
            }
            else
            {
                foreach (var sensorData in sensorDataDictionary.Values.Where(v => !v.Disabled))
                {
                    Console.WriteLine($"Still with id: {sensorData.StillId}. Avg. mash temp: {sensorData.AverageMashTemperatureReading} C°. Avg. distillate temp: {sensorData.AverageDistillateTemperatureReading} C°. Gas cpty.: {sensorData.LastGasCapacityPercentage}%");
                    Console.WriteLine("----------");

                    //TODO: Send alerts
                    if (sensorData.HasRealiableReadings && (sensorData.AverageDistillateTemperatureReading > 95 || sensorData.AverageMashTemperatureReading > 95))
                    {
                        

                    }
                }
            }
        }
    }
}
