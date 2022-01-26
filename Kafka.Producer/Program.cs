using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;

namespace Kafka.Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var topicName = "deneme.brk.qu";//Environment.GetEnvironmentVariable("TOPIC_NAME");
            var kafkaUrl = "localhost:9092"; //Environment.GetEnvironmentVariable("KAFKA_URL");

            var config = new ProducerConfig() { BootstrapServers = kafkaUrl };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                while (true)
                {
                    Console.Write("Enter message: ");
                    var text = Console.ReadLine();

                    Message<string, string> message = new Message<string, string> { Value = text };
                    DeliveryResult<string, string> result = producer.ProduceAsync(topicName, message).GetAwaiter().GetResult();
                    Console.WriteLine($"Delivered to '{result.TopicPartitionOffset}'");
                }
            }
        }
    }
}
