using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using System.IO;
using System.Net;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;

namespace OutfitForecastBot
{
    class Program
    {
        private static string token = "7060461024:AAH_wiOd5I8ksoATT-wxKuCS3FBQll6PRoE";
        private static TelegramBotClient bot = new TelegramBotClient(token);
        static string NameCity;
        static float tempOfCity;
        static string nameOfCity;
        static string answerOnWeather;

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }

        public static void Weather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=2351aaee5394613fc0d14424239de2bd";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                return;
            }
        }

        public static void Celsius(float celsius)
        {
            if (celsius <= 10)
                answerOnWeather = "Сегодня холодно одевайся потеплее!";
            else
                answerOnWeather = "Сегодня очень жарко, так что можешь одеть маечку и шортики.";
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, 
            Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, 
                        "Бот по прогнозу одежды готов к вашим услугам!");
                    return;
                }
                if (message.Type == MessageType.Text)
                {
                    NameCity = message.Text;
                    Weather(NameCity);
                    Celsius(tempOfCity);
                    await bot.SendTextMessageAsync(message.Chat.Id, $"{answerOnWeather} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Пожалуйста, введите любую команду.");
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, 
            Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
}