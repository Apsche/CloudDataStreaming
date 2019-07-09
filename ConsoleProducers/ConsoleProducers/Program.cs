using Confluent.Kafka;
using System;
using System.Timers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ConsoleProducers
{
    class Program
    {
        private static KafkaClient _client = new KafkaClient();
        private static string _configPath;
        private static string _certDir;
        private static ClientConfig _config;

        static async Task Main(string[] args)
        {

            if (args.Length != 2) { PrintUsage(); }

            _configPath = args[0];
            _certDir = args[1];

            // Load the Kafka Client configuration so that messages can be streamed
            _config = await _client.LoadConfig(_configPath, _certDir);

            // Set timer to send to producer
            Timer sendToProducer = new Timer(5000);
            sendToProducer.Elapsed += OnTimedEvent;
            sendToProducer.Enabled  = true;
            sendToProducer.AutoReset = true;

            Console.WriteLine("\nPress the 'Enter' key to kill the application.");
            Console.WriteLine($"\nThe application started at {DateTime.Now}");
            Console.ReadLine();
            sendToProducer.Stop();
            sendToProducer.Dispose();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            string topic = "testTopic";
            string key = "testKey";
            string value = JObject.FromObject(new { time = e.SignalTime.ToLongTimeString() }).ToString(Formatting.None);
            _client.Produce(topic, _config, key, value);
        }

        static void PrintUsage()
        {
            Console.WriteLine("usage: dotnet run <configPath> <certDir>");
            System.Environment.Exit(1);
        }
    }
}
