using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.PoliceDetector
{
    class Program
    {
        public static readonly ConsumerConfig ConsumerConfig = new ConsumerConfig
        {
            GroupId = nameof(Bimbrownia.AI.PoliceDetector),
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        static async Task Main(string[] args)
        {
            Console.Title = "Police Detector";
            Task.Run(MonitorStillPing);

            Console.ReadLine();
        }

        private static async Task MonitorStillPing()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(nameof(PoliceDetectedEvent));

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

                            var ev = JsonConvert.DeserializeObject<PoliceDetectedEvent>(cr.Value);
                            await HandlePoliceDetectedEvent(ev);
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

        private static async Task HandlePoliceDetectedEvent(PoliceDetectedEvent ev)
        {
            var currentBGColor = ConsoleColor.Blue;
            var nextBGColor = ConsoleColor.Red;

            for (int i = 0; i < 20; i++)
            {
                Console.BackgroundColor = currentBGColor;
                currentBGColor = nextBGColor;
                nextBGColor = Console.BackgroundColor;
                Console.Clear();
                await Task.Delay(200); 
            }

            Console.ResetColor();
            Console.Clear();
        }
    }
}
