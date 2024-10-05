using Telegram.Bot;
using Telegram.Bot.Types;

namespace Olymp_am
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await UpdateHelper.StartUpdateManagement(UpdateHelper.OlympAmURL, "Olymp Website");
        }
    }

    internal static class HTTPCaller
    {
        static HttpClient client = new HttpClient();
        public static HttpResponseMessage GET(string uri)
        {
            return client.Send(new HttpRequestMessage(new HttpMethod("GET"), uri));
        }

        public static string GET_AsText(string url)
        {
            return GET(url).Content.ReadAsStringAsync().Result;
        }
    }

    internal static class TelegramBotHelper
    {
        static TelegramBotClient client = new TelegramBotClient("7563844816:AAHby629vM4Zp5fyMSECeIfyQmX2CSgqlnQ");
        static int me = 1009729817;
        static TelegramBotHelper()
        {
            StartProcessing();
        }

        private static void StartProcessing()
        {
            client.StartReceiving(UpdateHandler, ErrorHandler);
        }

        private static async Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(me, "Crashed!");
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                try
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Please do not send me messages!");
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(me, "Crashed!");
                }
            }
        }

        public static async Task SendMeTextMessage(string message)
        {
            await SendTextMessage(me, message);
        }

        public static async Task SendTextMessage(int chatID, string messageText)
        {
            await client.SendTextMessageAsync(chatID, messageText);
        }
    }

    internal static class FileHelper
    {
        public static void WriteToFile(string path, string text)
        {
            using StreamWriter writer = new StreamWriter(path);
            writer.WriteLine(ReadFromFile(path) + '\n' + text);
        }

        public static string ReadFromFile(string path)
        {
            using StreamReader reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        public static void ReWriteInFile(string path, string text)
        {
            using StreamWriter reader = new StreamWriter(path);
            reader.WriteLine(text);
        }
    }

    internal static class UpdateHelper
    {
        public static string OlympAmURL = @"https://www.olymp.am/olympiads/all/all/all";
        public static async Task StartUpdateManagement(string url, string name)
        {
            string path = name + ".txt";
            FileHelper.ReWriteInFile(path, HTTPCaller.GET_AsText(url));
            while (true)
            {
                var html = FileHelper.ReadFromFile(path);
                var response = HTTPCaller.GET_AsText(url);
                if (response.Equals(html))
                {
                    FileHelper.ReWriteInFile(path, response);
                    await TelegramBotHelper.SendMeTextMessage($"Something changed the website \"{name}\"");
                }

                Thread.Sleep(10000);
            }
        }
    }
}
