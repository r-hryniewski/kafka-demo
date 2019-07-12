using Bimbrownia.AI.Shared;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public static class Kafka
    {
        private static readonly ProducerConfig ProducerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };


        public static readonly IProducer<Null, string> JsonMsgProducer = new ProducerBuilder<Null, string>(ProducerConfig).Build();

        public static void ProduceEventAsJson<TEvent>(string topicName, TEvent ev) where TEvent : Event
        {
            var serializedEvent = JsonConvert.SerializeObject(ev);
            JsonMsgProducer.Produce(topic: topicName,
                message: new Message<Null, string> { Value = serializedEvent });
        }
    }
}
