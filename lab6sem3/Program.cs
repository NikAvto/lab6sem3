using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public float Temp { get; set; }
    public string Description { get; set; }
}

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private const string apiKey = "9f4660ad344c504c11b271050a052132";

    static async Task Main(string[] args)
    {
        List<Weather> weatherData = new List<Weather>();
        Random random = new Random();
        double firstlat = 32.11122;
        double firstlon = 73.59228;

        for (int i = 0; i < 50; i++)
        {
            double latitude = firstlat + random.NextDouble() * 2;
            double longitude = firstlon + random.NextDouble() * 2;

            var weather = await GetWeatherAsync(latitude, longitude);
            if (weather != null)
            {
                weatherData.Add((Weather)weather); // Добавляем данные о погоде в список
                Console.WriteLine($"Город: {weatherData[i].Name}, Страна: {weatherData[i].Country}, Температура: {weatherData[i].Temp}°C, Описание: {weatherData[i].Description}");
            }
        }

        if (weatherData.Any())
        {
            var maxTempCountry = weatherData.OrderByDescending(w => w.Temp).First();
            var minTempCountry = weatherData.OrderBy(w => w.Temp).First();
            Console.WriteLine($"\nСтрана с максимальной температурой: {maxTempCountry.Country} ({maxTempCountry.Temp}°C)");
            Console.WriteLine($"Страна с минимальной температурой: {minTempCountry.Country} ({minTempCountry.Temp}°C)");
            var averageTemp = weatherData.Average(w => w.Temp);
            Console.WriteLine($"\nСредняя температура в мире: {averageTemp}°C");
            var uniqueCountriesCount = weatherData.Select(w => w.Country).Distinct().Count();
            Console.WriteLine($"\nКоличество стран в коллекции: {uniqueCountriesCount}");
            var firstMatchingDescription = weatherData.First(w => w.Description == "clear sky" || w.Description == "rain" || w.Description == "few clouds");
            if (firstMatchingDescription.Name != null)
            {
                Console.WriteLine($"\nПервая найденная страна: {firstMatchingDescription.Country}, Местность: {firstMatchingDescription.Name}, Описание: {firstMatchingDescription.Description}");
            }
            else
            {
                Console.WriteLine("\nНет найденных местностей с описаниями 'clear sky', 'rain' или 'few clouds'.");
            }
        }
        else
        {
            Console.WriteLine("Нет данных о погоде для анализа.");
        }
    }

    private static async Task<Weather?> GetWeatherAsync(double latitude, double longitude)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";

        try
        {
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);

            if (json["sys"] != null && json["name"] != null && json["main"] != null && json["weather"] != null)
            {
                return new Weather
                {
                    Country = json["sys"]["country"].ToString(),
                    Name = json["name"].ToString(),
                    Temp = float.Parse(json["main"]["temp"].ToString()),
                    Description = json["weather"][0]["description"].ToString()
                };
            }

            Console.WriteLine($"Данные о погоде недоступны для координат: {latitude}, {longitude}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении данных о погоде: {ex.Message}");
        }

        return null;
    }
}