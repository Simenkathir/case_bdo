using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        public static async Task getData()
        {
            // Build the configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            // Retrieve the API key from the configuration
            string apiKey = configuration["TwelveData:ApiKey"];
            string baseUrl = "https://api.twelvedata.com/";

            // Create HTTP client and set headers
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                string endpoint = "cryptocurrencies";

                HttpResponseMessage response = await client.GetAsync($"{endpoint}?apikey={apiKey}");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Cryptocurrency Data: ");

                    JsonObject data = (JsonObject)JsonNode.Parse(responseData)!;

                    await StoreData(data, configuration);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task StoreData(JsonObject data, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("sqlServer");

            if (string.IsNullOrEmpty(connectionString)){
                throw new InvalidOperationException("ConnectionString property has not been initialized. ");
            }

            Console.WriteLine($"Connection String: {connectionString}");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                JsonArray cryptocurrencies = (JsonArray)data["data"]!;

                foreach (JsonObject crypto in cryptocurrencies)
                {
                    string symbol = (string)crypto["symbol"]!;
                    string availableExchanges = string.Join(",", crypto["available_exchanges"]!.AsArray());
                    string currencyBase = (string)crypto["currency_base"]!;
                    string currencyQuote = (string)crypto["currency_quote"]!;

                    string query = "INSERT INTO Cryptocurrencies (Symbol, AvailableExchanges, CurrencyBase, CurrencyQuote) VALUES (@Symbol, @AvailableExchanges, @CurrencyBase, @CurrencyQuote)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Symbol", symbol);
                        command.Parameters.AddWithValue("@AvailableExchanges", availableExchanges);
                        command.Parameters.AddWithValue("@CurrencyBase", currencyBase);
                        command.Parameters.AddWithValue("@CurrencyQuote", currencyQuote);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        static async Task Main(string[] args)
        {
            await getData();
        }
    }
}
