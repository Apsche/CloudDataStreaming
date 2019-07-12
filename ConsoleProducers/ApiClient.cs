using System;
using System.Net.Http;
using System.Threading.Tasks;
using OpenWeatherMap;
using OpenWeatherMap.Entities;
using OpenWeatherMap.Util;

namespace ConsoleProducers
{
    class ApiClient
    {
        private static HttpClient _client = new HttpClient();
        private static KafkaClient _kafkaClient = new KafkaClient();

        public async Task<string> GetTrafficData(string configPath, string point, string zoom = "10", string style = "absolute")
        {
            var config = await _kafkaClient.ConfigToDictionary(configPath);
            var key = config["traffic.key"];
            string rootPath = "https://api.tomtom.com/traffic/services/4/flowSegmentData";
            string trafficData = null;

            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{rootPath}/{style}/{zoom}/json?key={key}&point={point}&unit=mph");
                response.EnsureSuccessStatusCode();
                trafficData = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nTraffic API failed with message: {e.Message}");
            }

            return trafficData;
        }

        public async Task<Weather> GetWeatherData(string configPath, string city) 
        {
            var config = await _kafkaClient.ConfigToDictionary(configPath);
            string key = config["weather.key"];

            var weatherService = new OpenWeatherMapService(new OpenWeatherMapOptions
            {
                ApiKey = key
            });
            RequestOptions.Default.Unit = UnitType.Imperial;

            Weather currentWeather = null;

            try
            {
                currentWeather = await weatherService.GetCurrentWeatherAsync(city);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nWeather API failed with message: {e.Message}");
            }

            return currentWeather;
        }


    }
}