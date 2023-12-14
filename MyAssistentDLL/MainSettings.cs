using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace MyAssistentDLL
{
    public class Account
    {
        private static readonly string NAME_TO_SAVE_FILE = $"{MainSettings.PATH}\\Accounts.json";
        public string Login { get; set; }
        public string Password { get; set; }
        private bool isActive = false;
        [JsonIgnore]
        public bool Connection => isActive;
        public void OpenCoonnection() => isActive = true;
        public void CloseCoonnection() => isActive = false;
        public static void CloseConnection(string Login)
        {
            foreach (var account in Accounts)
                if (account.Login.Equals(Login))
                {
                    account.isActive = false;
                    return;
                }
                    
        }
        public static List<Account> Accounts = new List<Account>();
        public static void RemoveID(int id)=>Accounts.RemoveAt(id);
        public static void Load()
        {
            Accounts = File.Exists(NAME_TO_SAVE_FILE) ? JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(NAME_TO_SAVE_FILE)) : new List<Account>();
            Console.WriteLine(Accounts);
        }
        public static void Save()
        {
            if (!Directory.Exists(NAME_TO_SAVE_FILE)) 
                Directory.CreateDirectory(NAME_TO_SAVE_FILE.Split('\\').First());

            File.WriteAllText(NAME_TO_SAVE_FILE, JsonConvert.SerializeObject(Accounts));
        }
        public static bool Add(string l, string p)
        {
            foreach (var item in Accounts)
                if (item.Login.Equals(l))
                    return false;

            Accounts.Add(new Account(l, p));
            return true;
        }
        public static Account FindUser(string l,string p)
        {
            foreach (var item in Accounts)
                if (item.Login == l && item.Password == p.GetHashCode().ToString())
                    return item;
            throw new Exception("No Finded User");
        }
        public Account()
        {

        }
        Account(string l,string p)
        {
            Login = l;  
            Password = p.GetHashCode().ToString();
        }
        public override string ToString() => $"{Login}: {isActive}";
    }
    public static class MainSettings
    {
        private static Timer tm;

        public static TimeSpan SaveCommandTime = new TimeSpan(0, 0, 30);
        public static TimeSpan WaitForConectionDevice = new TimeSpan(0, 0, 30);
        public static readonly string PATH = "Saves";
        private static readonly string NAME = $"{PATH}\\Settings.json";
        public static readonly string PATH_SAVE_SCRIPT = $"Script";
        public static double Sensivity = 0.5;

        //CheckAutoSave
        public static string SpeechCulture = "ru-Ru";
        public static string VoiceCulture = "";
        //SwitchVoiceLog
        public static bool VoiceLog = false;
        //SwitchVoiceMessage
        public static bool VoiceMessage  = true;

        //========<Com>
        //BaudRateComPort
        public static int BaudRate  = 9600;
        //NameComPort
        public static string COMname = "COM5";
        //=======>

        //=======<Server>
        public static TimeSpan CheckConnectServer = new TimeSpan(0, 0, 30);
        public static int PortServer = 11000;
        public static int MaxUsers = 20;
        public static string IPServer = "192.168.1.200";
        public static int AddresListIDServer = 2;
        //========>

        //==========<Network>
            public static string SSID = "MYSSID";
            public static string PASSWORLD = "MYPASS";
        //========>

        //==========<Local Web Server>
        public static bool StatusWebServer = false;
        public static string MainPrefix = "http://localhost:7878/";
        public static string MainFolder = @"C:\Users\User\source\repos\MyAsistent\MyAsistent\Module\Internet\WebServer\Site";
        //==========>

        //==========<Inject>
        public static bool StatusInject = true;
        public static string IpInject = "127.0.0.1";
        public static int PortInject = 2745;

        //==========>

        //======<Telegram Module>
        //StatusTelegramBot
        public static bool StatusTBot = false;
        public static string APIKey = "";
        public static List<long> WhiteListID = new List<long>()
        {

        };
        //==========>

        //======<Script>
        //==========>


        public static Dictionary<string,string> CultureText = new Dictionary<string,string>();
        public class Saves
        {




            public  bool StatusInject = MainSettings.StatusInject;
            public  string IpInject = MainSettings.IpInject;
            public  int PortInject = MainSettings.PortInject;
            public  bool StatusTBot = MainSettings.StatusTBot;
            public  string APIKey = MainSettings.APIKey;
            public  double Sensivity = MainSettings.Sensivity;
            public List<long> WhiteListID = MainSettings.WhiteListID;



            public  string SpeechCulture = MainSettings.SpeechCulture;
            public  string VoiceCulture = MainSettings.VoiceCulture;
            public  bool VoiceLog = MainSettings.VoiceLog;
            public  int BaudRate = MainSettings.BaudRate;
            public  int MaxUsers = MainSettings.MaxUsers;
            public TimeSpan SaveCommandTime = MainSettings.SaveCommandTime;
            public TimeSpan WaitForConectionDevice = MainSettings.WaitForConectionDevice;
            public string COMname = MainSettings.COMname;
            public  bool VoiceMessage = MainSettings.VoiceMessage;
            public int PortServer = MainSettings.PortServer;
            public string IPServer = MainSettings.IPServer;
            public string SSID = MainSettings.SSID;
            public string PASSWORLD = MainSettings.PASSWORLD;
            

            public string MainPrefix = MainSettings.MainPrefix;
            public bool StatusWebServer = MainSettings.StatusWebServer;
            public string MainFolder = MainSettings.MainFolder;


            public int AddresListIDServer = MainSettings.AddresListIDServer;
            public Dictionary<string, string> CultureText = MainSettings.CultureText;


            


            public Saves()
            {

            }
            
        }
        public static void Save() {
            Module.Codes.CodeSaver.Save();
            if (!Directory.Exists(NAME)) Directory.CreateDirectory(NAME.Split('\\')[0]);
            File.WriteAllText(NAME, JsonConvert.SerializeObject(new Saves()));
            Account.Save();
            //Windows.Code.CodePage.Save();
        }
        public static void Load()
        {
            Account.Load();
            var save = File.Exists(NAME) ? JsonConvert.DeserializeObject<Saves>(File.ReadAllText(NAME)) : new Saves();
            SpeechCulture = save.SpeechCulture;
            Sensivity = save.Sensivity;
            VoiceCulture = save.VoiceCulture;
            VoiceLog = save.VoiceLog;
            VoiceMessage = save.VoiceMessage;
            CultureText = save.CultureText;
            COMname = save.COMname;
            BaudRate = save.BaudRate;
            MaxUsers = save.MaxUsers;
            PortServer = save.PortServer;
            IPServer = save.IPServer;
            AddresListIDServer = save.AddresListIDServer;
            PASSWORLD = save.PASSWORLD;
            SSID = save.SSID;
            StatusWebServer = save.StatusWebServer;
            MainFolder = save.MainFolder;
            MainPrefix = save.MainPrefix;

            StatusInject = save.StatusInject;
            IpInject = save.IpInject;
            PortInject = save.PortInject;
            StatusTBot = save.StatusTBot;
            APIKey = save.APIKey;
            WhiteListID = save.WhiteListID;

       


        // WaitForConectionDevice = save.WaitForConectionDevice;
        // SaveCommandTime = save.SaveCommandTime;
    }

        public static void Init()
        {
            if (tm != null)
            {
                tm.Change(SaveCommandTime, Timeout.InfiniteTimeSpan);
            }
            else
            {
                tm = new Timer(TimerCallback, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
            }
        }

        private static void TimerCallback(object state)
        {
            MyAssistentDLL.Module.Codes.CodeSaver.Save();
            Save();
            Logs.Log.Write(Logs.TypeLog.Message, $"Успешное сохранение в {DateTime.Now.ToLongTimeString()} ");
            tm.Change(SaveCommandTime, Timeout.InfiniteTimeSpan);
        }


    }
}
