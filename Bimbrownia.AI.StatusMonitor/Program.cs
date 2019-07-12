using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.StatusMonitor
{
    class Program
    {
        public static readonly ConsumerConfig ConsumerConfig = new ConsumerConfig
        {
            GroupId = nameof(Bimbrownia.AI.StatusMonitor),
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        private static readonly ConcurrentDictionary<Guid, StillStatus> stillStatusDictionary = new ConcurrentDictionary<Guid, StillStatus>();
        static async Task Main(string[] args)
        {
            Console.Title = "Status Monitor";

            Task.Run(MonitorStillStarted);
            Task.Run(MonitorStillDisabled);
            Task.Run(MonitorStillPing);

            while (true)
            {
                Console.Clear();
                RenderStatus();
                await Task.Delay(1000);
            }
        }
        private static async Task MonitorStillStarted()
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

        private static async Task MonitorStillPing()
        {
            using (var c = new ConsumerBuilder<Ignore, string>(ConsumerConfig).Build())
            {
                c.Subscribe(Constants.TopicName_StillPing);

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

                            var ev = JsonConvert.DeserializeObject<StillPingEvent>(cr.Value);
                            HandleStillPingEvent(ev);
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

        private static void HandleStillPingEvent(StillPingEvent ev)
        {
            stillStatusDictionary.AddOrUpdate(key: ev.StillId,
                addValue: new StillStatus()
                {
                    Id = ev.StillId,
                    Started = ev.Occured,
                    LastSeen = ev.Occured
                },
                updateValueFactory: (stillId, item) =>
                {
                    return new StillStatus()
                    {
                        Id = item.Id,
                        Started = item.Started,
                        LastSeen = ev.Occured > item.LastSeen ? ev.Occured : item.LastSeen,
                        Disabled = item.Disabled
                    };
                });
        }

        private static void HandleStillDisabledEvent(StillDisabledEvent ev)
        {
            stillStatusDictionary.AddOrUpdate(key: ev.StillId,
                addValue: new StillStatus()
                {
                    Id = ev.StillId,
                    Started = ev.Occured,
                    LastSeen = ev.Occured
                },
                updateValueFactory: (stillId, item) =>
                {
                    return new StillStatus()
                    {
                        Id = item.Id,
                        Started = item.Started,
                        LastSeen = ev.Occured > item.LastSeen ? ev.Occured : item.LastSeen,
                        Disabled = ev.Occured
                    };

                });
        }

        private static void HandleStillStartedEvent(StillStartedEvent ev)
        {
            stillStatusDictionary.AddOrUpdate(key: ev.StillId,
                addValue: new StillStatus()
                {
                    Id = ev.StillId,
                    Started = ev.Occured,
                    LastSeen = ev.Occured
                },
                updateValueFactory: (stillId, item) =>
                {
                    return new StillStatus()
                    {
                        Id = item.Id,
                        Started = ev.Occured,
                        LastSeen = ev.Occured > item.LastSeen ? ev.Occured : item.LastSeen
                    };

                });
        }

        private static void RenderStatus()
        {
            Console.WriteLine($"----------BIMBROWNIA AI----------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
            Console.WriteLine("Systems status:");

            if (stillStatusDictionary.IsEmpty)
            {
                Console.WriteLine("No stills running");
            }
            else
            {
                foreach (var stillStatus in stillStatusDictionary.Values)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (stillStatus.IsRunning && DateTime.Now - stillStatus.LastSeen > TimeSpan.FromMinutes(1))
                    {
                        Console.WriteLine($"Still with id: '{stillStatus.Id}' Seems to be disconnected. Started at: {stillStatus.Started}. Last seen: {stillStatus.LastSeen}.");
                    }
                    else
                    {
                        Console.ForegroundColor = stillStatus.IsRunning ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine($"Still with id: '{stillStatus.Id}' is {(stillStatus.IsRunning ? "running" : "not running")}. Started at: {stillStatus.Started}. Last seen: {stillStatus.LastSeen}.{(stillStatus.IsRunning ? "" : $" Stopped at: {stillStatus.Disabled}")} ");
                    }
                    Console.ResetColor();
                }
            }
        }
    }
}
