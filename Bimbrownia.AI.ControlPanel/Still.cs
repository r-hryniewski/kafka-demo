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

        public bool GasTankIsEmpty => gasCapacity <= 0;

        private int gasCapacity;

        public Still()
        {
            Id = Guid.NewGuid();
            gasCapacity = 100;
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
            int iterationsToRefill = 10;
            while (!Cts.Token.IsCancellationRequested)
            {
                if (!GasTankIsEmpty)
                    gasCapacity -= 1;

                Kafka.ProduceEventAsJson(Constants.TopicName_StillPing, new StillPingEvent(Id));

                var r = new Random();

                var randomMashTemperature = Temperatures.TemperaturesValues[r.Next(0, Temperatures.TemperaturesValues.Length)];
                var randomDistillateTemperature = Temperatures.TemperaturesValues[r.Next(0, Temperatures.TemperaturesValues.Length)];



                Kafka.ProduceEventAsJson(topicName: Constants.TopicName_StillSensorRead,
                    ev: new StillSensorReadEvent(Id, randomMashTemperature, randomDistillateTemperature, gasCapacity));

                await Task.Delay(1000);

                if (GasTankIsEmpty)
                {
                    iterationsToRefill -= 1;

                    if (iterationsToRefill <= 0)
                    {
                        gasCapacity = 100;
                        iterationsToRefill = 10;
                    }
                }
            }
        }
    }
}