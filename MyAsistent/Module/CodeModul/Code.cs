using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Speech.Synthesis;
using System.Windows.Forms;
using System;
using System.Data.SqlTypes;
using MyAsistent.Module.MessageBoxes;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace Codes
{
    public delegate void Close(object sender);
    public interface MenuArgumentItem
    {
        (TypeArgumend, string[], bool local)? Result { get; }
        void clear();
        object Content { get; set; }
        event Close OnClose;
    }
    public enum TypeArgumend
    {
        Path,
        Keyboard,
        Command,
        Voice,
        Arduino,
        ArduinoReadDate
    }
    /*
     Краткая памятка для Сергея)
    Path из аргументов только 0 индекс который хранит в себе путь к исполняемому файлу \\Поддерживает выполнение на удалённом пк

    Keyboard аргумент это массив кнопок кототорые нужно прожать одновременно. \\Поддерживает выполнение на удалённом пк

    Command емуляция команд для командной строки   \\Поддерживает выполнение на удалённом пк

    Voice аргумент только один это строка 

    Arduino 0 Индекс Имя платы 1 Комманда которую нужно отправить на арудино 3 то что нужно сказать после выполнения

    ArduinoReadDate 0 Индекс Имя платы 1 Комманда которую нужно отправить на арудино 3 то что нужно сказать после выполнения пренципиальная разница между прошлой версией в том что мы будем ждать ответ от ардуинки
    и в случае его получение в нужные места будут подставленны нужные данные
     
     */

    public static class COMSENDER
    {
        static SerialPort _serialPort;
        public static ObservableCollection<string> GetAllCom() => new ObservableCollection<string>(SerialPort.GetPortNames());
        public static void INIT()
        {
            try
            {
                _serialPort = new SerialPort(MyAsistent.MainSettings.COMname, MyAsistent.MainSettings.BaudRate, Parity.None, 8, StopBits.One);
                Logs.Log.Write(Logs.TypeLog.Message, $"Trying to open Serial Port: { _serialPort.PortName}");

                _serialPort.Open();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, $"Trying to open Serial Port: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, $"Trying to open Serial Port: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, $"Trying to open Serial Port: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, $"Trying to open Serial Port: {ex.Message}");
            }
            catch(Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, $"Trying to open Serial Port: {ex.Message}");

            }
        }
        public static void WriteToSerialPort(string message)
        {

            _serialPort.Write(message);
        }

        
    }

    public class Code_Saver
    {
        private readonly static string _path = @"Saves\SaveCommand.json";
        public static ObservableCollection<Code_Saver> dates_Culture = new ObservableCollection<Code_Saver>();
        public static List<Code_Saver> dates = new List<Code_Saver>();
        public static int CountCommand { get => dates.Count; }
        public (string,string) commands;
        public List<(TypeArgumend, string[], bool local)> Arguments;
        public override string ToString() => $"{commands} | <{Arguments.Count}> ";
        public static void add((string, string) command, List<(TypeArgumend, string[], bool local)> arguments)
        {
            if (interpretatot.values.TryGetValue(command, out var value)) return;
            var Codes = new Code_Saver() { Arguments = arguments, commands = command };
            dates.Add(Codes);
            dates_Culture.Add(Codes);
            interpretatot.values.Add(command, new Code(arguments));
            MyAsistent.Module.Sound.Sound.add_string(command.Item1);
        }

        public class Code
        {
            List<(TypeArgumend, string[], bool local)> Arguments;

            public Code(List<(TypeArgumend, string[], bool local)> arguments)
            {
                Arguments = arguments;
            }
            private void emulatedKeyboard(MyAsistent.Properties.KeyboardEmu obj)
            {
                if (obj.keys == null || obj.keys.Count == 0) return;
                if (obj.keys.Count > 1)
                {
                    string command = "";
                    foreach (var key in obj.keys)
                    {
                        switch (key)
                        {
                            case System.Windows.Input.Key.LeftShift:
                            case System.Windows.Input.Key.RightShift:
                                command += "+";
                                break;
                            case System.Windows.Input.Key.LeftCtrl:
                            case System.Windows.Input.Key.RightCtrl:
                                command += "^";
                                break;
                            case System.Windows.Input.Key.LeftAlt:
                            case System.Windows.Input.Key.RightAlt:
                                command += "%";
                                break;
                            case System.Windows.Input.Key.F1:
                            case System.Windows.Input.Key.F2:
                            case System.Windows.Input.Key.F3:
                            case System.Windows.Input.Key.F4:
                            case System.Windows.Input.Key.F5:
                            case System.Windows.Input.Key.F6:
                            case System.Windows.Input.Key.F7:
                            case System.Windows.Input.Key.F8:
                            case System.Windows.Input.Key.F9:
                            case System.Windows.Input.Key.F10:
                            case System.Windows.Input.Key.F11:
                            case System.Windows.Input.Key.F12:
                                command += "{" + key.ToString() + "}";
                                break;
                            default: command += key.ToString(); break;
                        }
                    }
                    SendKeys.SendWait(command);
                }
                else SendKeys.SendWait(obj.keys[0].ToString());
            }
            
            private static  Dictionary<char, Func<string, string>> CharsSymbols = new Dictionary<char, Func<string, string>>()
            {
                {'$',new Func<string,string>(str=>DateTime.Now.ToLongDateString()) },
                {'@',new Func<string,string>(str=>DateTime.Now.ToLongTimeString()) }
            };
            private string BuildSoundString(string input)
            {
                for (int i = 0; i < CharsSymbols.Count; i++) {
                    while (true)
                    {
                        var index = input.ToCharArray().ToList().FindIndex(p => p == CharsSymbols.ElementAt(i).Key);
                        if (index >= 0)
                        {
                           
                            input = input.Insert(index, CharsSymbols[CharsSymbols.ElementAt(i).Key].Invoke(input));
                            input = input.Remove(input.ToCharArray().ToList().FindIndex(p => p == CharsSymbols.ElementAt(i).Key), 1);
                        }
                        else break;
                    } 
                }
                return input;
            }
            
            private bool IneterpretatorArduinoDate(string input,string ArduinoDate,out string result)
            {
                try
                {
                    var array = input.Split(' ');
                    var Input = ArduinoDate.Split('_');
                    for (int i = 0; i < array.Length; i++)
                        if (array[i].Length > 0) 
                            if (array[i][0] == '[' && array[i][array[i].Length - 1] == ']')
                                for (int j = 0; j < Input.Length; j++)
                                    if (Input[j].Contains(array[i]))
                                    {
                                        array[i] = Input[j].Split(':')[1];
                                    }
                    result = string.Join(" ", array);
                    return true;
                }
                catch (Exception)
                {
                    result = input;
                    return false;
                }
               
                

                
                
            }
            class SendToPC
            {
                public string Name;
                public string[] Args;

                public SendToPC(string name, string[] args)
                {
                    Name = name;
                    Args = args;
                }
            }
            public async void Start()
            {
                foreach(var Arguments in Arguments)
                {
                    if (!Arguments.local)
                    {
                        //Реализовать отправку по ком порту или по веб а не только по веб как сейчас
                        MyAsistent.Module.Internet.InternetWorkerModule.SendToClent(Arguments.Item2[0], JsonConvert.SerializeObject(new SendToPC(Arguments.Item1.ToString(),Arguments.Item2)));
                        continue;
                    }
                    switch (Arguments.Item1)
                    {
                        
                        case TypeArgumend.Path:
                            foreach (var i in Arguments.Item2)
                                Process.Start(new ProcessStartInfo(i));
                            break;
                        case TypeArgumend.Voice:
                            foreach (var item in Arguments.Item2)
                                MyAsistent.Module.Sound.Voice.PlayRuAsync(BuildSoundString(item));
                            break;
                        case TypeArgumend.Arduino:
                            MyAsistent.Module.Internet.InternetWorkerModule.SendToClent(Arguments.Item2[0], Arguments.Item2[1]);
 
                            break;
                        case TypeArgumend.ArduinoReadDate:
                            await System.Threading.Tasks.Task.Run(() =>
                            {
                                if (MyAsistent.Module.Internet.InternetWorkerModule.SendToClinetRecive(Arguments.Item2[0], Arguments.Item2[1], out string output))
                                {
                                    if (Arguments.Item2[2].Length != 0 &&
                                        IneterpretatorArduinoDate(Arguments.Item2[2], output, out string voices))
                                        MyAsistent.Module.Sound.Voice.PlayRuAsync(voices);
                                }
                            });
                           
                            break;

                        case TypeArgumend.Keyboard:
                            foreach (var i in Arguments.Item2)
                            {
                                if (MyAsistent.Properties.KeyboardEmu.tryParse(i, out MyAsistent.Properties.KeyboardEmu variable))
                                    emulatedKeyboard(variable);
                                else continue;
                            }
                            break;
                        case TypeArgumend.Command:
                            break;
                        default:
                            break;
                    }
                }
                    
            }
           
        }
        public static void ChangeCulture(string culture)
        {
            MyAsistent.Module.Sound.Sound.Change_Culture(culture);
            MyAsistent.MainSettings.SpeechCulture = culture;
            dates_Culture.Clear();
            LoadGrammar();

        }
        private static void LoadGrammar()
        {
            interpretatot.values.Clear();
            MyAsistent.Module.Sound.Sound.ClearItems();
            foreach (var i in dates)
            {
                if (i.commands.Item2.ToLower() == MyAsistent.MainSettings.SpeechCulture.ToLower())
                {
                    MyAsistent.Module.Sound.Sound.add_string(i.commands.Item1);
                    interpretatot.values.Add(i.commands, new Code(i.Arguments));
                    dates_Culture.Add(i);
                }
                    
            }
        }
        public static void Load()
        {
            dates = File.Exists(_path)?JsonConvert.DeserializeObject<List<Code_Saver>>(File.ReadAllText(_path)):new List<Code_Saver>();
            LoadGrammar();
        }
        public static void Save()
        {
            if (!Directory.Exists(_path)) Directory.CreateDirectory(_path.Split('\\')[0]);
            File.WriteAllText(_path, JsonConvert.SerializeObject(dates));
        }
         
    }
    
    public class interpretatot
    {
        public static Dictionary<(string, string), Code_Saver.Code> values = new Dictionary<(string, string), Code_Saver.Code>();

        

        public static void Run(string command)
        {
            if (values.ContainsKey((command,MyAsistent.MainSettings.SpeechCulture)))
            {
                values[(command, MyAsistent.MainSettings.SpeechCulture)].Start();
                Logs.Log.Write(Logs.TypeLog.Message, $"{MyAsistent.MainSettings.CultureText[MyAsistent.MainSettings.SpeechCulture]} |{command}|");
                if(MyAsistent.MainSettings.VoiceMessage&&!MyAsistent.MainSettings.VoiceLog)
                    MyAsistent.Module.Sound.Voice.PlayRuAsync($"{MyAsistent.MainSettings.CultureText[MyAsistent.MainSettings.SpeechCulture]} {command}");
            }
               
        }
    }
}
