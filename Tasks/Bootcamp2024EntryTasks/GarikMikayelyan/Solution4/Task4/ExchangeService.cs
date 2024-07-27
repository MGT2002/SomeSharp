using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace Task4
{
    class ExchangeService : IExchangeService
    {
        private readonly HttpClient httpClient;
        private const string ApiKey = "fca_live_nInwEzYO05ezlL4MPNblFDVzTIylGuTtQ0DaIQjC";

        public ExchangeService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", ApiKey);
        }

        public async Task<Dictionary<string, decimal>> GetCurrenciessAsync()
        {
            try
            {
                HttpResponseMessage response = await
                    httpClient.GetAsync("https://api.freecurrencyapi.com/v1/latest");
                var jsonData = await response.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<JsonModel>
                    (jsonData)!.Data;

                return currencies;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return [];
            }
        }

        public async Task<decimal> ExchangeAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            try
            {
                string url = $"https://api.freecurrencyapi.com/v1/latest?apikey={ApiKey}" +
                    $"&currencies={toCurrency.ToUpperInvariant()}" +
                    $"&base_currency={fromCurrency.ToUpperInvariant()}";

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<JsonModel>(jsonResponse)!.Data;

                return amount * data[toCurrency.ToUpperInvariant()];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing real-time exchange: {0}", ex.Message);
                return 0;
            }
        }

        public async Task<decimal> HistoricalExchangeAsync(string fromCurrency, string toCurrency, decimal amount, DateTime date)
        {
            try
            {
                string url = $"https://api.freecurrencyapi.com/v1/historical?apikey=" +
                    $"{ApiKey}&currencies={toCurrency.ToUpperInvariant()}&" +
                    $"base_currency={fromCurrency.ToUpperInvariant()}&" +
                    $"date_from={date}&date_to={date}";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonTextReader reader = new(new StringReader(jsonResponse));

                double k = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
                    {
                        k = (double)reader.Value!;
                        break;
                    }
                }

                return amount * Convert.ToDecimal(k);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error performing historical exchange: {0}", ex.Message);
                return 0;
            }
        }
    }
}
