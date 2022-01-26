using Confluent.Kafka;
using System;
using System.Linq;

namespace KafkaConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var consumergroup = "cs";//Environment.GetEnvironmentVariable("CONSUMER_GROUP");
            var topicName = "deneme.brk.qu";//Environment.GetEnvironmentVariable("TOPIC_NAME");
            var brokerList = "localhost:9092"; //Environment.GetEnvironmentVariable("KAFKA_URL");


            var config = new ConsumerConfig { GroupId = consumergroup, BootstrapServers = brokerList };

            using (var consumer = new ConsumerBuilder<string, string>(config)
                .SetPartitionsAssignedHandler((c, partitions) => {
                    Console.WriteLine(
                        "Partitions incrementally assigned: [" +
                        string.Join(',', partitions.Select(p => p.Partition.Value)) +
                        "], all: [" +
                        string.Join(',', c.Assignment.Concat(partitions).Select(p => p.Partition.Value)) +
                        "]");
                    //else Console.WriteLine($"Revoked partitions: [{string.Join(", ", e.Partitions)}]");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                    // assignment is incremental (may remove only some partitions of the current assignment).
                    var remaining = c.Assignment.Where(atp => partitions.Where(rtp => rtp.TopicPartition == atp).Count() == 0);
                    Console.WriteLine(
                        "Partitions incrementally revoked: [" +
                        string.Join(',', partitions.Select(p => p.Partition.Value)) +
                        "], remaining: [" +
                        string.Join(',', remaining.Select(p => p.Partition.Value)) +
                        "]");
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    // The lost partitions handler is called when the consumer detects that it has lost ownership
                    // of its assignment (fallen out of the group).
                    Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(topicName);
                while (true)
                {
                    ConsumeResult<string, string> consumeResult = consumer.Consume();
                    Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                    consumer.Commit();
                }
            }
        }
    }
}
