using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp{
    class Program {

        public static async Task getData(){

            // Build the configuration
            var builder = new ConfigurationBuilder().
            SetBasePath(AppContext.BaseDirectory).
            AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            // Retrieve the API key from the configuration
            string apiKey = configuration["TwelveData:ApiKey"];
            string baseUrl = "https://api.twelvedata.com/";
            
            // Create HTTP client and set headers
            using (HttpClient client = new HttpClient()) {

                client.BaseAddress = new Uri(baseUrl);

                string endpoint = "cryptocurrencies";

                HttpResponseMessage response = await client.GetAsync($"{endpoint}?apikey={apiKey}");

                if (response.IsSuccessStatusCode) {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Cryptocurrency Data: ");
                    Console.WriteLine(responseData); 
                } else {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

        }
        static async Task Main(string[] args) {
            await getData();
        }
    }
}
