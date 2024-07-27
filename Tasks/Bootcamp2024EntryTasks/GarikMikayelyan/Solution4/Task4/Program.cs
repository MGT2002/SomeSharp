using System.Globalization;
using Task4;

var exchangeService = new ExchangeService();

while (true)
{
    Console.WriteLine("Выберите действие:");
    Console.WriteLine("1 - Вывести все доступные валюты;");
    Console.WriteLine("2 - Сделать обмен одной валюты в другую;");
    Console.WriteLine("3 - Сделать обмен одной валюты в другую на указанную дату в прошлом;");

    if (!int.TryParse(Console.ReadLine(), out var choice))
    {
        Console.WriteLine("Некорректный ввод");
        continue;
    }

    switch (choice)
    {
        case 1:
            var currencies = await exchangeService.GetCurrenciessAsync();
            Console.WriteLine("Доступные валюты:");
            foreach (var currency in currencies)
            {
                Console.WriteLine(currency);
            }
            break;
        case 2:
            Console.Write("Введите код валюты из которой осуществляется обмен: ");
            var fromCurrency = Console.ReadLine();

            Console.Write("Введите код валюты в которую осуществляется обмен: ");
            var toCurrency = Console.ReadLine();

            Console.Write("Введите число для обмена: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount))
            {
                Console.WriteLine("Некорректный ввод");
                continue;
            }

            var exchangedAmount = await exchangeService.ExchangeAsync(fromCurrency!, toCurrency!, amount);
            Console.WriteLine($"Результат обмена: {exchangedAmount} {toCurrency}");
            break;
        case 3:
            Console.Write("Введите код валюты из которой осуществляется обмен: ");
            fromCurrency = Console.ReadLine();

            Console.Write("Введите код валюты в которую осуществляется обмен: ");
            toCurrency = Console.ReadLine();

            Console.Write("Введите число для обмена: ");
            if (!decimal.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("Некорректный ввод");
                continue;
            }

            Console.Write("Введите дату в формате ГГГГ-ММ-ДД: ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                Console.WriteLine("Некорректный ввод даты");
                continue;
            }

            exchangedAmount = await exchangeService.HistoricalExchangeAsync(fromCurrency!, toCurrency!, amount, date);
            Console.WriteLine($"Результат обмена: {exchangedAmount} {toCurrency}");
            break;
        default:
            Console.WriteLine("Некорректный выбор");
            break;
    }

    Console.WriteLine("Для выхода введите 'exit', для продолжения нажмите Enter");
    if (Console.ReadLine() == "exit")
    {
        break;
    }
}
