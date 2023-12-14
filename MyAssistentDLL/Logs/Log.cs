using System;
using System.IO;
using System.Linq;
using System.Threading;


namespace MyAssistentDLL.Logs
{
    public class Log
    {
        private static Mutex Mutext = new Mutex();
        private static Mutex MutextWeb = new Mutex();
        public static readonly string PATH = "Logs\\Log.log";
        public static readonly string PATH_TO_WEB_LOG = "Logs\\WebServerLog.log";
        public  delegate void SendMsgToConsole(TypeLog type, string message);
        public static event SendMsgToConsole SendMsgToConsoleEvent;
        public static event SendMsgToConsole SendMsgToConsoleWebServerEvent;

        private static void CheckPath(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path.Split('\\').First());
            if (!File.Exists(path)) File.Create(path);
        }
        public static void Write(TypeLog type, string message)
        {
            if (MainSettings.VoiceLog)
                Module.Sound.Voice.PlayRuAsync(message);
            CheckPath(PATH);
            Mutext.WaitOne();
            while (true)
            {
                try
                {
                    File.AppendAllText(PATH, $"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | {type}\n");
                    break;
                }
                catch (IOException ex)
                {
                    Write(TypeLog.Error, ex.Message);
                }
            }
            Mutext.ReleaseMutex();
            SendMsgToConsoleEvent?.Invoke(type,message);

        }
        public static void WriteWeb(TypeLog type, string message)
        {
            if (MainSettings.VoiceLog)
                Module.Sound.Voice.PlayRuAsync(message);
            CheckPath(PATH_TO_WEB_LOG);
            MutextWeb.WaitOne();
            while (true)
            {
                try
                {
                    File.AppendAllText(PATH_TO_WEB_LOG, $"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | {type}\n");
                    break;
                }
                catch (IOException ex)
                {
                    Write(TypeLog.Error, ex.Message);
                }
            }
            MutextWeb.ReleaseMutex();
            SendMsgToConsoleWebServerEvent?.Invoke(type, message);
        }
    }
}
