using Microsoft.Speech.Recognition;
using MyAssistentDLL.Module.CodeModul.Compiler;
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL.Module.Internet;
using MyAssistentDLL.Module.Internet.CodeInject;
using MyAssistentDLL.Module.Internet.WebServer;
using MyAssistentDLL.Module.Sound;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using static MyAssistentDLL.Module.Internet.WebServer.WebServerManager;

namespace MyAssistentDLL
{
    public partial class ControllerAssistent
    {
        public static event UpdateSettings OnUpdateSettings = EventUpdateSettings;

        public static void InilizeAssistent()
        {

            MainSettings.Init();
            MainSettings.Load();
            Sound.Init();
            Voice.Init();

            COMSENDER.INIT();

            CodeSaver.Load();

            WebServerManager.Init();
            InternetWorkerModule.StartServer();



        }

        //Injection
        internal static InjectionServer InjectionServer = new InjectionServer();
        public static void StartInjectionServer()=> InjectionServer.StartServer();
        public static void StopInjectionServer() => InjectionServer.CloseServer();
        //TelegramBot
        public static void StartTelegramBot() => Module.Internet.TelegramBotModule.TelegramBot.Start();
        public static void StopTelegramBot() => Module.Internet.TelegramBotModule.TelegramBot.Stop();
        //WebServer
        public static void StartWebServer() => Start();
        public static void StopWebServer() => Close();
        //intertnet 

        public static event ConnectionEvent OnConnetctedDevicesInternet = InternetWorkerModule.ConnectorHelper.ConnectedEvent;

        public static void RestartInternetWorker()=> InternetWorkerModule.StartServer();
        public static void RestartInternetWorkerDispetcher() => InternetWorkerModule.DispetcherClassesConection.Start();

        public static string UpdateOTAArduino(string Path,IInternetClient Client)
        {
            var arduinoClient = Client as ArduinoClient;
            return arduinoClient.OTAUpdate(Path);
        }
        public static ObservableCollection<string> GetAllCommandArduino(IInternetClient Client)
        {
            var arduinoClient = Client as ArduinoClient;
            return arduinoClient.AllCommand;
        }
        public static ObservableCollection<string> GetRecCommandArduino(IInternetClient Client)
        {
            var arduinoClient = Client as ArduinoClient;
            return arduinoClient.CommandRec;
        }
        public static ObservableCollection<string> GetCommandArduino(IInternetClient Client)
        {
            var arduinoClient = Client as ArduinoClient;
            return arduinoClient.Command;
        }
        public static List<IInternetClient> GetAllArduinoClinet() => InternetWorkerModule.DispetcherClassesConection.ArduinoClients.ConvertAll(new Converter<ArduinoClient, IInternetClient>((x) => x));
        //Voice

        public static double VoiceSensivity { get => Sound.Sensivity; set => Sound.Sensivity = value; }
        public static void SetVoiceSpeech(string CultureName)=> Voice.SelectVoice(CultureName);
        public static string[] AllVoiceCultureName() => Voice.GetName();
        public static IReadOnlyCollection<RecognizerInfo> AllSoundCultureName() => Sound.getAllCultures();

        //COM
        public static ObservableCollection<string> GetAllCom() => COMSENDER.GetAllCom();
        public static void RestartCom()=> COMSENDER.INIT();

        //Code
        public static ObservableCollection<CodeSaver> GetCurrentLangCulture() => CodeSaver.dates_Culture;
        public static void ChangeCulture(string NewCultureName) => CodeSaver.ChangeCulture(NewCultureName);
        public static string[] GetAllNameTypeArgument() => Enum.GetNames(typeof(TypeArgumend));

        public static bool CompileCode(string Code, out List<ErrorInfo> Error, out MemoryStream Stream) => CodeCompiler.Compile(Code, out Error, out Stream);
        public static void RunCode(MemoryStream Stream)=> CodeCompiler.Run(Stream);
    




}
}
