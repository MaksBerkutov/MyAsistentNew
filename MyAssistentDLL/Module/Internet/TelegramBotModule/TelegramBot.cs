using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using System.Collections.ObjectModel;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using MyAssistentDLL.Logs;

namespace MyAssistentDLL.Module.Internet.TelegramBotModule
{
    internal static class TelegramBot
    {
      

        private static Dictionary<string, ObservableCollection<(Codes.TypeArgumend, string[], bool local)>> CustomScenaries = new Dictionary<string, ObservableCollection<(Codes.TypeArgumend, string[], bool local)>>();



        static ITelegramBotClient bot;
        private delegate  void MenuItemSelected(ITelegramBotClient botClient, Message msg);
       
        static private (string, MenuItemSelected)[][] Menu = new (string, MenuItemSelected)[][]
        {
            new (string, MenuItemSelected)[]{("Емуляция комманд ⚙️", EmulationCMD),("Базовая информация 📋", BaseInfo)},
            new (string, MenuItemSelected)[]{("Логи приложения 🛠️", GetFileLog),("Логи сайта 🌐", GetFileWebLog) }
        };
        private async static void EmulationCMD(ITelegramBotClient botClient, Message msg)
        {
            List<InlineKeyboardButton> tmpKey = new List<InlineKeyboardButton>();
            List<List<InlineKeyboardButton>> tmpKey1 = new List<List<InlineKeyboardButton>>();
            foreach (var item in Codes.CodeSaver.dates_Culture)
                tmpKey1.Add(new List<InlineKeyboardButton>() { new InlineKeyboardButton(item.commands.Item1) { CallbackData = $"EMC_{item.commands.Item1}" } });
            await botClient.SendTextMessageAsync(msg.Chat, "Все комманды", ParseMode.Html, null, true, false, null, 0, null, new InlineKeyboardMarkup(tmpKey1.ToArray()));
        }
        private async static void GetFileLog(ITelegramBotClient botClient, Message msg)
        {
            while (true)
            {
                try
                {
                    using (var stream = System.IO.File.OpenRead(Log.PATH))
                    {
                        string[] tmp = System.IO.File.ReadAllLines(Log.PATH);
                        Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream);
                        iof.FileName = "Log.txt";
                        var send = await botClient.SendDocumentAsync(msg.Chat, iof, "Log Dump");
                    }
                    break;
                }
                catch (System.IO.IOException ex)
                {
                    Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                }
            }
           
        }
        private async static void GetFileWebLog(ITelegramBotClient botClient, Message msg)
        {
            while (true)
            {
                try
                {
                    using (var stream = System.IO.File.OpenRead(Log.PATH_TO_WEB_LOG))
                    {
                        string[] tmp = System.IO.File.ReadAllLines(Log.PATH_TO_WEB_LOG);
                        Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream);
                        iof.FileName = "WebLog.txt";
                        var send = await botClient.SendDocumentAsync(msg.Chat, iof, "Web Log Dump");
                    }
                    break;
                }
                catch (System.IO.IOException ex)
                {
                    Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                }
            }

        }
        private async static void BaseInfo(ITelegramBotClient botClient, Message msg)
        {
            string Status = $"==<MainStatus>==\n" +
                $"Веб Сервер: {(MainSettings.StatusWebServer ? "Запущен✅" : "Остановлен❌")}\n" +
                $"Иньекции:  {(MainSettings.StatusInject ? "Разрещенны✅" : "Запрещенны❌")}\n" +
                $"\n==<Ip>==\n" +
                $"Ip Inject:  {MainSettings.IpInject}:{MainSettings.PortInject}\n" +
                $"Ip Main:  {MainSettings.IPServer}:{MainSettings.PortServer}\n" +
                $"Ip WebServer: {MainSettings.MainPrefix}\n" +
                $"\n==<Com Settings>==\n" +
                $"Bhaud Rate: {MainSettings.BaudRate}\n" +
                $"COM: {MainSettings.COMname}\n" +
                $"\n==<Server>==\n" +
                $"Count Connected Devices: {InternetWorkerModule.DispetcherClassesConection.ALLDevice.Count}\n" +
                $"Of Arduino: {InternetWorkerModule.DispetcherClassesConection.ArduinoClients.Count}\n";
            await botClient.SendTextMessageAsync(msg.Chat, Status);

        }




        private static bool ThisIsMenuItem(ITelegramBotClient botClient, Message msg)
        {
            foreach(var item in Menu)
                foreach(var item2 in item)
                    if(item2.Item1 == msg.Text)
                    {
                        item2.Item2(botClient, msg);
                        return true;
                    }
            return false;
        }
        private static bool isUser(long id)
        {
            foreach(var i in MainSettings.WhiteListID)
            {
                if (i.Equals(id))
                    return true;
            }
            
            return false;   
        }
        public static KeyboardButton[][] CreateMenu()
        {
            KeyboardButton[][] result  = new KeyboardButton[Menu.Length][];
           for(var i = 0; i < result.Length; i++)
            {
                var tmp = new List<KeyboardButton>();
                foreach(var item in Menu[i]) tmp.Add(item.Item1);
                result[i] = tmp.ToArray();
            }
            return result;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message)
            {

                var message = update.Message;
                if (isUser(message.From.Id))
                {
                    if (message.Text.ToLower() == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "Menu", ParseMode.Html, null, true, false, null, 0, null, new ReplyKeyboardMarkup(CreateMenu()));
                        return;
                    }
                    else if (ThisIsMenuItem(botClient, message)) return;
                  
                }
                else if (message.Text.ToLower() == "/id") await botClient.SendTextMessageAsync(message.Chat, $"User Name: {message.From.Username}\nUser ID: {message.From.Id}");



            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                CallbackQuery callbackQuery = update.CallbackQuery;
                if(callbackQuery.Data.Split('_')[0]=="EMC")
                {
                    Codes.interpretatot.Run( callbackQuery.Data.Split('_')[1]);
                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        { 
            await Task.Run(() => Log.Write(TypeLog.Error, exception.Message));
        }
        private static CancellationTokenSource cts;
        public static  void Stop()
        {
            if(cts!= null)
                cts.Cancel();
            bot = null;
        }
        public static void Start()
        {
            bot = new TelegramBotClient(MainSettings.APIKey);
            cts = new CancellationTokenSource();
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
        }
    }
   
}
