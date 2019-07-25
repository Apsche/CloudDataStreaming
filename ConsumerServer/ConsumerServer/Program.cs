using System;
using System.Threading;
using Confluent.Kafka;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ConsumerServer
{
    public class Program
    {
        private static KafkaClient _kafkaClient = new KafkaClient();
        private static string _configPath;
        private static string _certDir;
        public static void Main(string[] args)
        {
            _configPath = args[0];
            _certDir = args[1];

            try
            {
                var t = new Thread(new ThreadStart(KafkaConsumer));
                t.Start();
            }

            catch (Exception e)
            {
                Console.WriteLine("oops, the consumer didn't work");
                Console.WriteLine(e.Message);
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

       private static void KafkaConsumer()
        {
            var config = _kafkaClient.LoadConfig(_configPath, _certDir).Result;
            _kafkaClient.Consume("Output", config);
            Console.WriteLine("Success");
        }
    }
}
