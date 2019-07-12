using Confluent.Kafka;
using System;

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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
