
using Newtonsoft.Json;

class StockInfo
{
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public double Change { get; set; }
    public double ChangePercent { get; set; }
}

class Program
{
    static void Main()
    {
        List<string> tickers = new() { "AAPL", "GOOGL", "MSFT" };
        GetStockInformation(tickers);
    }

    static void GetStockInformation(List<string> tickers)
    {
        string apiKey = "ZLY2356Z0R1VHZL1";
        string apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&apikey={apiKey}&symbol=";

        using (HttpClient client = new())
        {
            foreach (string ticker in tickers)
            {
                string requestUrl = apiUrl + ticker;

                HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;

                    // Parse the JSON response
                    dynamic? result = JsonConvert.DeserializeObject(json);
                    dynamic quote = result["Global Quote"];

                    if (quote != null)
                    {
                        StockInfo stockInfo = new()
                        {
                            Symbol = ticker,
                            Name = quote["01. symbol"],
                            Price = Convert.ToDouble(quote["05. price"]),
                            Change = Convert.ToDouble(quote["09. change"]),
                            ChangePercent = ParsePercentage(quote["10. change percent"].ToString())
                        };

                        Console.WriteLine("Stock: {0} ({1})", stockInfo.Name, stockInfo.Symbol);
                        Console.WriteLine("Price: ${0}", stockInfo.Price);
                        Console.WriteLine("Change: ${0} ({1}%)", stockInfo.Change, stockInfo.ChangePercent);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve stock information for {0}.", ticker);
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Failed to retrieve stock information for {0}.", ticker);
                    Console.WriteLine();
                }
            }
        }
        Console.ReadLine();
    }

    static double ParsePercentage(string percentage)
    {
        percentage = percentage.TrimEnd('%');
        return Convert.ToDouble(percentage);
    }
}
