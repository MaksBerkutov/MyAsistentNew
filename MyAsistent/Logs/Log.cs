﻿using System;
using System.IO;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Media;

namespace Logs
{
    class Log
    {
        private static Mutex mutext = new Mutex();
        private static Mutex mutextWeb = new Mutex();
        public static readonly string Path = "Logs\\Log.log";
        public static readonly string PathToWebLog = "Logs\\WebServerLog.log";
        public  delegate void SendMsgToConsole(Paragraph para);
        public static event SendMsgToConsole SendMsgToConsoleEvent;
        public static event SendMsgToConsole SendMsgToConsoleWebServerEvent;
        public static void Write(TypeLog type, string message)
        {
            MyAsistent.App.Current.Dispatcher.Invoke(() =>
            {

            
            try
            {
                if (MyAsistent.MainSettings.VoiceLog)
                    MyAsistent.Module.Sound.Voice.PlayRuAsync(message);
                if (!Directory.Exists(Path)) Directory.CreateDirectory(Path.Split('\\')[0]);

                if (!File.Exists(Path)) File.Create(Path);
                mutext.WaitOne();
                while (true)
                {
                    try
                    {
                        File.AppendAllText(Path, $"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | {type.ToString()}\n");
                        break;
                    }
                    catch (System.IO.IOException ex)
                    {
                        Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                    }
                }
                mutext.ReleaseMutex();



                Paragraph para = new Paragraph();
                para.Inlines.Add(new Run($"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | ") { Foreground = new SolidColorBrush(Colors.LightGreen) });
                switch (type)
                {
                    case TypeLog.Message:
                        para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Green) }));
                        break;
                    case TypeLog.Error:
                        para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Red) }));
                        break;
                    case TypeLog.Warning:
                        para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Yellow) }));
                        break;

                }
                //textBox.Document.Blocks.Add(para)
                SendMsgToConsoleEvent?.Invoke(para);
            }
            catch (Exception ex)
            {
                //Logs.Log.Write(Logs.TypeLog.Error, ex.Message);

            }
            });

        }
        public static void WriteWeb(TypeLog type, string message)
        {
            MyAsistent.App.Current.Dispatcher.Invoke(() =>
            {

                try
                {
                    if (MyAsistent.MainSettings.VoiceLog)
                        MyAsistent.Module.Sound.Voice.PlayRuAsync(message);
                    if (!Directory.Exists(PathToWebLog)) Directory.CreateDirectory(Path.Split('\\')[0]);

                    if (!File.Exists(PathToWebLog)) File.Create(Path);
                    mutextWeb.WaitOne();
                    while (true)
                    {
                        try
                        {
                            File.AppendAllText(PathToWebLog, $"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | {type.ToString()}\n");
                            break;
                        }
                        catch (System.IO.IOException ex)
                        {
                            Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                        }
                    }
                    mutextWeb.ReleaseMutex();

                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Run($"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | ") { Foreground = new SolidColorBrush(Colors.LightGreen) });
                    switch (type)
                    {
                        case TypeLog.Message:
                            para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Green) }));
                            break;
                        case TypeLog.Error:
                            para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Red) }));
                            break;
                        case TypeLog.Warning:
                            para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Yellow) }));
                            break;
                        case TypeLog.Graphics:
                            para = new Paragraph();
                            para.Inlines.Add(new Bold(new Run(message) { Foreground = new SolidColorBrush(Colors.Green) }));
                            break;

                    }

                    SendMsgToConsoleWebServerEvent?.Invoke(para);
                }
                catch (Exception ex)
                {

                    Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                }
            });


        }
    }
}
