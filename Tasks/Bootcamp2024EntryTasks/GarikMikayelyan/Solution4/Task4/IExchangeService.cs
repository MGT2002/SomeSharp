namespace Task4
{
    interface IExchangeService
    {
        Task<Dictionary<string, decimal>> GetCurrenciessAsync();
        Task<decimal> ExchangeAsync(string fromCurrency, string toCurrency, decimal amount);
        Task<decimal> HistoricalExchangeAsync(string fromCurrency, string toCurrency, decimal amount, DateTime date);
    }
}
