using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.ControlPanel
{
    internal class Still
    {
        public Guid Id { get; }
        private CancellationTokenSource Cts { get; set; }
        private Task Task { get; set; }

        private static readonly Random r = new Random();

        public Still()
        {
            Id = Guid.NewGuid();
        }

        internal void Run()
        {
            Cts = new CancellationTokenSource();

            var stillStartedEvent = new StillStartedEvent(Id);
            Kafka.ProduceEventAsJson(Constants.TopicName_StillStarted, stillStartedEvent);

            Task = Task.Run<Task>(
                function: ProduceRandomSensorData,
                cancellationToken: Cts.Token);
        }

        internal async Task DisableAsync()
        {
            Cts.Cancel();
            await Task;
            Task.Dispose();

            var stillDisabledEvent = new StillDisabledEvent(Id);
            Kafka.ProduceEventAsJson(Constants.TopicName_StillDisabled, stillDisabledEvent);
        }

        private async Task ProduceRandomSensorData()
        {
            while (!Cts.Token.IsCancellationRequested)
            {
                Kafka.ProduceEventAsJson(Constants.TopicName_StillPing, new StillPingEvent(Id));

                var randomTemperature = Temperatures.TemperaturesValues[r.Next(0, Temperatures.TemperaturesValues.Length)];

                Kafka.ProduceEventAsJson(topicName: Constants.TopicName_TemperatureSensor1,
                    ev: new TemperatureReadEvent(Id, randomTemperature));

                await Task.Delay(500);
            }
        }
    }
}