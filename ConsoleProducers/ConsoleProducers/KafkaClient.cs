using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleProducers
{
    public class KafkaClient
    {
        public async Task<ClientConfig> LoadConfig(string configPath, string certDir)
        {
            try
            {
                var cloudConfig = await ConfigToDictionary(configPath);
                
                // = (await File.ReadAllLinesAsync(configPath))
                //     .Where(line => !line.StartsWith("#"))
                //     .ToDictionary(
                //         line => line.Substring(0, line.IndexOf('=')),
                //         line => line.Substring(line.IndexOf('=') + 1));

                var clientConfig = new ClientConfig
                {
                    BootstrapServers = cloudConfig["bootstrap.servers"].Replace("\\", ""),
                    SaslMechanism = SaslMechanism.Plain,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslUsername = cloudConfig["sasl.username"],
                    SaslPassword = cloudConfig["sasl.password"]
                };

                if (certDir != null)
                {
                    clientConfig.SslCaLocation = certDir;
                }

                return clientConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured reading the config file from '{configPath}': {e.Message}");
                System.Environment.Exit(1);
                return null; // avoid not-all-paths-return-value compiler error.
            }
        }

        public async Task<Dictionary<string, string>> ConfigToDictionary(string path)
        {
           return (await File.ReadAllLinesAsync(path))
                    .Where(line => !line.StartsWith("#"))
                    .ToDictionary(
                        line => line.Substring(0, line.IndexOf('=')),
                        line => line.Substring(line.IndexOf('=') + 1));
        }

        public async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
        {
            using (var adminClient = new AdminClientBuilder(cloudConfig).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = name, NumPartitions = numPartitions, ReplicationFactor = replicationFactor } });
                }
                catch (CreateTopicsException e)
                {
                    if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine("Topic already exists");
                    }
                }
            }
        }

        public void Produce(string topic, ClientConfig config, string key = "KEY", string value = "{\"value\":\"something\"}")
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                var sendMessageTask = producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value });
                sendMessageTask.ContinueWith((task) =>
                {
                    if (!task.IsCompletedSuccessfully)
                    {
                        Console.WriteLine($"Failed to deliver message: {task.Exception.Message}");
                    }
                    else 
                    {
                        Console.WriteLine($"Sent message to {topic} at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}");
                    }
                });

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }

        public void Consume(string topic, ClientConfig config)
        {
            var consumerConfig = new ConsumerConfig(config);
            consumerConfig.GroupId = "dotnet-example-group-1";
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            consumerConfig.EnableAutoCommit = false;

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                var totalCount = 0;
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        totalCount += JObject.Parse(cr.Value).Value<int>("count");
                        Console.WriteLine($"Consumed record with key {cr.Key} and value {cr.Value}, and updated total count to {totalCount}");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        

    }
}
