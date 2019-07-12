using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.AlertsMonitor
{
    class Program
    {
        public static readonly ConsumerConfig ConsumerConfig = new ConsumerConfig
        {
            GroupId = nameof(Bimbrownia.AI.AlertsMonitor),
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        static async Task Main(string[] args)
        {
            Console.Title = "Alerts Monitor";
            Console.WriteLine($"----------BIMBROWNIA AI----------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");

            Task.Run(MonitorStillStarted);
            Task.Run(MonitorStillDisabled);

            Console.ReadLine();
        }

        private static void MonitorStillStarted()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(Constants.TopicName_StillStarted);

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

                            var ev = JsonConvert.DeserializeObject<StillStartedEvent>(cr.Value);
                            HandleStillStartedEvent(ev);
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

        private static void MonitorStillDisabled()
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

        private static void HandleStillStartedEvent(StillStartedEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} started at {ev.Occured:yyyy/MM/dd hh:mm:ss.fff}", ConsoleColor.Green);
        }

        private static void HandleStillDisabledEvent(StillDisabledEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} disabled at {ev.Occured:yyyy/MM/dd hh:mm:ss.fff}", ConsoleColor.Red);
        }
    }
}
