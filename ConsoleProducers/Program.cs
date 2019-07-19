using Confluent.Kafka;
using System;
using System.Timers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleProducers
{
    class Program
    {
        private static KafkaClient _client = new KafkaClient();
        private static ApiClient _apiClient = new ApiClient();
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

            // Set timer to send traffic data to producer
            Timer sendTrafficToProducer = new Timer(5000);
            sendTrafficToProducer.Elapsed += OnTrafficTimedEvent;
            sendTrafficToProducer.Enabled  = true;
            sendTrafficToProducer.AutoReset = true;

            // Set timer to send weather data to producer
            Timer sendWeatherToProducer = new Timer(5000);
            sendWeatherToProducer.Elapsed += OnWeatherTimedEvent;
            sendWeatherToProducer.Enabled = true;
            sendWeatherToProducer.AutoReset = true;

            // _client.Consume("Traffic", _config);

            Console.WriteLine("\nPress the 'Enter' key to kill the application.");
            Console.WriteLine($"\nThe application started at {DateTime.Now}");
            Console.ReadLine();

            // Stop timers
            sendTrafficToProducer.Stop();
            sendWeatherToProducer.Stop();

            // Dispose timers
            sendTrafficToProducer.Dispose();
            sendWeatherToProducer.Dispose();
        }

        private static void OnTrafficTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get traffic data on 271 near Campus II
            string currentTraffic = _apiClient.GetTrafficData(_configPath, "41.57505,-81.44750").Result;
            _client.Produce("Traffic", _config, "mayfield", currentTraffic);
        }
        private static void OnWeatherTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get current weather conditions in Cleveland
            string currentWeather = _apiClient.GetWeatherData(_configPath, "Cleveland").Result;
            _client.Produce("Weather", _config, "mayfield", currentWeather);
        }

        static void PrintUsage()
        {
            Console.WriteLine("usage: dotnet run <configPath> <certDir>");
            System.Environment.Exit(1);
        }
    }
}
