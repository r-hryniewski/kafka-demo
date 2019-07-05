using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bimbrownia.AI.ControlPanel
{
    internal class Program
    {
        private const ConsoleKey quitButton = ConsoleKey.Q;
        private const ConsoleKey temperatureProducer1Button = ConsoleKey.D1;
        private const ConsoleKey temperatureProducer2Button = ConsoleKey.D2;
        private const ConsoleKey capacityProducer1Button = ConsoleKey.D3;
        private const ConsoleKey capacityProducer2Button = ConsoleKey.D4;
        private const ConsoleKey policeDetectorButton = ConsoleKey.D5;

        private static bool running = true;

        private static readonly Queue<Still> stills = new Queue<Still>();

        public static async Task Main(string[] args)
        {
            Console.Title = "Control Panel";

            while (running)
            {
                RenderMenu();
                var key = Console.ReadKey();
                await PerformAction(key.Key);
            }
        }

        private static void RenderMenu()
        {
            Console.WriteLine($"----------BIMBROWNIA AI----------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
            Console.WriteLine($"Stills running: {stills.Count}");
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("1: Add new still");
            Console.WriteLine("2: Add 10 new stills");
            Console.WriteLine("3: Add 100 new stills");
            Console.WriteLine("4: Disable still");
            Console.WriteLine("5: Disable 10 stills");
            Console.WriteLine("6: Disable 100 stills");
            Console.WriteLine("7: Disable 1000 stills");
            Console.WriteLine("0: Run police scanner");
            Console.WriteLine($"{quitButton}: Quit");
        }

        private static async Task PerformAction(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D1:
                    AddStills(1);
                    break;
                case ConsoleKey.D2:
                    AddStills(10);
                    break;
                case ConsoleKey.D3:
                    AddStills(100);
                    break;
                case ConsoleKey.D4:
                    await DisableStillsAsync(1);
                    break;
                case ConsoleKey.D5:
                    await DisableStillsAsync(10);
                    break;
                case ConsoleKey.D6:
                    await DisableStillsAsync(100);
                    break;
                case ConsoleKey.D7:
                    await DisableStillsAsync(1000);
                    break;
                case ConsoleKey.D0:
                    //TODO: 
                    Console.WriteLine("TODO");
                    break;
                case quitButton:
                    running = false;
                    break;
                default:
                    key = Console.ReadKey().Key;
                    await PerformAction(key);
                    break;
            }

            Console.Clear();
        }

        private static async Task DisableStillsAsync(int stillsNumber)
        {
            for (int i = 0; i < stillsNumber; i++)
            {
                if (stills.TryDequeue(out Still still))
                {
                    await still.DisableAsync();
                    //TODO: raise still disabled event
                }
            }
        }

        private static void AddStills(int stillsNumber)
        {
            for (int i = 0; i < stillsNumber; i++)
            {
                var still = new Still();
                still.Run();

                stills.Enqueue(still);
                //TODO: raise still created event
            }
        }
    }
}
