using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.AlertsMonitor
{
    internal class Program
    {
        public static readonly ConsumerConfig ConsumerConfig = new ConsumerConfig
        {
            GroupId = nameof(Bimbrownia.AI.AlertsMonitor),
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        private static async Task Main(string[] args)
        {
            Console.Title = "Alerts Monitor";
            Console.WriteLine($"----------BIMBROWNIA AI----------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");

            Task.Run(MonitorStillStarted);
            Task.Run(MonitorStillDisabled);
            Task.Run(MonitorDangerousDistillateAvgTemperature);
            Task.Run(MonitorDangerousMashAvgTemperature);
            Task.Run(MonitorGasTankIsEmpty);

            Console.ReadLine();
        }

        private static void MonitorDangerousDistillateAvgTemperature()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(nameof(DangerousDistillateAvgTemperatureEvent));

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

                            var ev = JsonConvert.DeserializeObject<DangerousDistillateAvgTemperatureEvent>(cr.Value);
                            HandleDangerousDistillateAvgTemperatureEvent(ev);
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

        private static void HandleDangerousDistillateAvgTemperatureEvent(DangerousDistillateAvgTemperatureEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} detected dangerous distillate temperature of: {ev.AverageDistillateTemperature} C°", ConsoleColor.DarkYellow);
        }

        private static void MonitorGasTankIsEmpty()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(nameof(GasTankIsEmptyEvent));

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

                            var ev = JsonConvert.DeserializeObject<GasTankIsEmptyEvent>(cr.Value);
                            HandleGasTankIsEmptyEvent(ev);
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

        private static void MonitorDangerousMashAvgTemperature()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(nameof(DangerousMashAvgTemperatureEvent));

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

                            var ev = JsonConvert.DeserializeObject<DangerousMashAvgTemperatureEvent>(cr.Value);
                            HandleDangerousMashAvgTemperatureEvent(ev);
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

        private static void MonitorStillStarted()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(nameof(StillStartedEvent));

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
                c.Subscribe(nameof(StillDisabledEvent));

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

        private static void HandleDangerousMashAvgTemperatureEvent(DangerousMashAvgTemperatureEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} detected dangerous mash temperature of: {ev.AverageMashTemperature} C°", ConsoleColor.DarkYellow);
        }

        private static void HandleStillStartedEvent(StillStartedEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} started at {ev.Occured:yyyy/MM/dd hh:mm:ss.fff}", ConsoleColor.Green);
        }

        private static void HandleStillDisabledEvent(StillDisabledEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} disabled at {ev.Occured:yyyy/MM/dd hh:mm:ss.fff}", ConsoleColor.Red);
        }

        private static void HandleGasTankIsEmptyEvent(GasTankIsEmptyEvent ev)
        {
            Misc.WriteLineInColor($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}: Still with id {ev.StillId} detected empty gas tank.", ConsoleColor.DarkCyan);
        }
    }
}
