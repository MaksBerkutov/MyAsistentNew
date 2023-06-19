using Mono.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//netsh http add iplisten 192.168.1.200
namespace MyAsistent.Module.Internet.WebServer
{
    static class WebServerManager
    {
        public interface IHttpsRespones
        {
            void Update(string Date);
            void StartHandle();
            string Get();
        }
        //Classe to send 
        public class WebServerSettings: IHttpsRespones{
            public bool check;
            public string path;
            public string Url;

            public WebServerSettings()
            {
                this.check = MainSettings.StatusWebServer;
                this.path = MainSettings.MainFolder;
                this.Url = MainSettings.MainPrefix;
            }

            public string Get()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle() => MainWindow.date.SyncronaizeWebGui();

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<WebServerSettings>(Date);
                MainSettings.StatusWebServer = this.check = item.check;
                MainSettings.MainFolder = this.path = item.path;
                MainSettings.MainPrefix = this.Url = item.Url;
            }
        }
        public class ServerSettings: IHttpsRespones
        {
            public string ip;
            public int port;
            public List<string> ips;
            public string  SelectedIps;
            public ServerSettings()
            {
                ip = MainSettings.IPServer;
                port = MainSettings.PortServer;
                ips = new List<string>();
                try
                {
                    Dns.GetHostEntry(MainSettings.IPServer).AddressList.ToList().ForEach(x => ips.Add(x.ToString()));
                    SelectedIps = ips[MainSettings.AddresListIDServer];
                }
                catch (Exception ex)
                {

                    Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);
                }
                
            }
            public string Get()
            {
                ips.Clear();
                try
                {
                    Dns.GetHostEntry(MainSettings.IPServer).AddressList.ToList().ForEach(x => ips.Add(x.ToString()));
                    SelectedIps = ips[MainSettings.AddresListIDServer];
                }
                catch (Exception ex)
                {

                    Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle() => MainWindow.date.SyncronaizeServerGui();

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerSettings>(Date);
                if (MainSettings.IPServer != item.ip)
                {
                    MainSettings.IPServer = this.ip = item.ip;
                    MainSettings.PortServer = this.port = item.port;
                    MainSettings.AddresListIDServer = 0;
                    ips = new List<string>();
                    try
                    {
                        Dns.GetHostEntry(MainSettings.IPServer).AddressList.ToList().ForEach(x => ips.Add(x.ToString()));
                        SelectedIps = ips.First();
                    }
                    catch (Exception ex)
                    {

                        Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);
                    }
                    
                }
                else
                {
                    MainSettings.PortServer = this.port = item.port;
                    MainSettings.AddresListIDServer = item.ips.FindIndex(x => x.ToString() == item.SelectedIps);
                    this.ips = item.ips;
                    this.SelectedIps = item.SelectedIps;
                }
                
            }

        }
        public class TelegramBotModule: IHttpsRespones
        {
            public bool check;
            public string api;
            public string id;

            public TelegramBotModule()
            {
                this.check = MainSettings.StatusTBot;
                this.api = MainSettings.APIKey;
                this.id = String.Empty;
            }

            public string Get()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle() => MainWindow.date.SyncronaizeTelegramGui();

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<TelegramBotModule>(Date);
               
                MainSettings.APIKey = this.api = item.api;

                MainSettings.StatusTBot = this.check = item.check;
                
                if (id != item.id && long.TryParse(item.id, out var Tid))
                {
                    id = item.id;
                    App.Current.Dispatcher.Invoke(() => MainSettings.WhiteListID.Add(Tid));
                   
                }



            }
        }
        public class InjectionModule : IHttpsRespones
        {
            public bool check;
            public string login;
            public string pass;

            public InjectionModule()
            {
                this.check = MainSettings.StatusInject;
                this.login = String.Empty;
                this.pass = String.Empty;
            }

            public string Get()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle() => MainWindow.date.SyncronaizeInjection();

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<InjectionModule>(Date);
                MainSettings.StatusInject = this.check = item.check;
                if (login != item.login && item.login.Any() && item.pass.Any()) 
                {
                    login = item.login;
                    App.Current.Dispatcher.Invoke(() => Account.Add(item.login, item.pass));

                }



            }
        }
        public class MainSettingsModule: IHttpsRespones
        {
            public bool VoiceMessage;
            public bool VoiceLog;
            public string CultureSpeech;
            public string CultureRecognition;
            public double timeAutoSave;
            public double timeWaitIniit;
            public List<string> AllSpeech = new List<string>();
            public List<string> AllRecognition = new List<string>();

            private bool Flag = false;
            public MainSettingsModule()
            {
                timeAutoSave = MainSettings.SaveCommandTime.TotalSeconds;
                timeWaitIniit = MainSettings.WaitForConectionDevice.TotalSeconds;
                CultureSpeech = MainSettings.SpeechCulture;
                CultureRecognition = MainSettings.VoiceCulture;
                VoiceMessage = MainSettings.VoiceMessage;
                VoiceLog = MainSettings.VoiceLog;
                Module.Sound.Voice.GetName().ToList().ForEach(x=>AllSpeech.Add(x));
                Module.Sound.Sound.getAllCultures().ToList().ForEach(x =>
                {
                    if(x.Culture.Name.Length != 0)
                        AllRecognition.Add(x.Culture.Name);
                }); 

            }

            public string Get()
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }

            public void StartHandle()
            {
                if(Flag)
                    MainWindow.MainWindow_Static.SyncronizateMainSeetings();
            }

            public void Update(string Date)
            {
                Flag = false;
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<MainSettingsModule>(Date);
                if (timeAutoSave != item.timeAutoSave)
                {
                    timeAutoSave = item.timeAutoSave;
                    MainSettings.SaveCommandTime = new TimeSpan(0,0, (int)timeAutoSave);
                    Flag =true;
                }
                if (timeWaitIniit != item.timeWaitIniit)
                {
                    timeWaitIniit = item.timeWaitIniit;
                    MainSettings.WaitForConectionDevice = new TimeSpan(0, 0, (int)timeWaitIniit);
                    Flag = true;
                }
                if (CultureSpeech != item.CultureSpeech)
                {
                    
                    MainSettings.SpeechCulture = CultureSpeech = item.CultureSpeech; 
                    Flag = true;
                }
                if (CultureRecognition != item.CultureRecognition)
                {

                    MainSettings.VoiceCulture = CultureRecognition = item.CultureRecognition; 
                    Flag = true;
                }
                if (VoiceMessage != item.VoiceMessage)
                {

                    MainSettings.VoiceMessage = VoiceMessage = item.VoiceMessage; 
                    Flag = true;
                }
                if (VoiceLog != item.VoiceLog)
                {

                    MainSettings.VoiceLog = VoiceLog = item.VoiceLog; 
                    Flag = true;
                }


             

            }
        }
        static List<IHttpsRespones> ListClasses = new List<IHttpsRespones>()
        {
            new WebServerSettings(),
            new MainSettingsModule(),
            new TelegramBotModule(),
            new InjectionModule(),
            new ServerSettings()
        };
        //End
        static HttpListener Listener = null;
        static readonly Dictionary<string, string> ConetneType = new Dictionary<string, string>
        {
            {"html", "text/html"},
            {"png", "image/png"},
            {"jpeg", "image/jpeg"},
            {"js", "text/javascript"},
            {"css", "text/css"},
            {"json", "application/json"},
            {"ico", "image/x-icon"},
            {"svg", "image/svg+xml"}
        };

        public static void Close()
        {
            try
            {
                if(Listener != null)
                    Listener.Stop();
              
               Logs.Log.WriteWeb(Logs.TypeLog.Message, "Веб сервер успешно выключен!");

            
              
               
            }
            catch (Exception ex)
            {

              
                    Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);
                

            }
        }
        public static async void Start()
        {
            await Task.Run(() =>
            {
                if (!HttpListener.IsSupported)
                {
                    Logs.Log.WriteWeb(Logs.TypeLog.Error, "HttpListener is not supported on this platform.");
                    MainSettings.StatusWebServer = false;
                    return;
                }
                try
                {
                    
                    Listener = new HttpListener();
                    
                        Listener.Prefixes.Add(MainSettings.MainPrefix);
                        Listener.Prefixes.Add(MainSettings.MainPrefix + "json/");
                        Listener.Start();

                        Listener.BeginGetContext(GetContextCallback, null);
                   
                        Logs.Log.WriteWeb(Logs.TypeLog.Message, $"Веб сервер успешно запущен ({MainSettings.MainPrefix}) !");
                      

                    
                        
                      
                    
                }
                catch (Exception ex)
                {
                   Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);

                }
            });
        }
        private static void GETorPOSTHandler(HttpListenerContext context)
        {
            //all comand format
            /*
            {
            action: 'NameAction' Update or Get
            name: Name Class Or Update
            date: JSON string date;
            }
             */
            try
            {
                var buffer = Encoding.UTF8.GetBytes("{ }");
                JObject MainObject = JObject.Parse(new StreamReader(context.Request.InputStream).ReadToEnd());
                //Logs.Log.WriteWeb(Logs.TypeLog.Message, $"Пришло с сервера  = {MainObject}");
                if (MainObject.Value<string>("action") == "update")
                {
                    var res = ListClasses.Find(p =>
                    p.GetType().Name.Equals(MainObject.Value<string>("name")
                    ));
                    Logs.Log.WriteWeb(Logs.TypeLog.Message, $"Запрос на обновление страници {res.GetType().Name}");
                    res.Update(MainObject.Value<string>("date"));
                    res.StartHandle();
                }
                else
                {
                    Logs.Log.WriteWeb(Logs.TypeLog.Message, $"Запрос на получение данных для страници {MainObject.Value<string>("name")}");
                    buffer = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ListClasses.
                                           Find(p =>
                                           {
                                               return p.GetType().Name.Equals(MainObject.Value<string>("name"));
                                           }
                                           ).Get()));
                }
                   

                context.Response.ContentType = ConetneType["json"];
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = 200;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                
                Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);
                
            }

            context.Response.OutputStream.Close();

        }
        private static string Convert(string input)
        {
            return HttpUtility.UrlDecode(input);
        }
        public static void OnNewRespone(HttpListenerContext context)
        {
            string RawUrl = context.Request.RawUrl;
            if (context.Request.RawUrl.Contains('%')) RawUrl = Convert(context.Request.RawUrl);
            if (RawUrl.Contains("json"))
            {
                GETorPOSTHandler(context); return;
            }
            byte[] buffer = null;
            var response = context.Response;

            if (RawUrl.Length > 2)
            {
               
                buffer = File.ReadAllBytes($@"{MainSettings.MainFolder}{RawUrl}");
                response.ContentType = ConetneType[context.Request.RawUrl.Split('.')[1]];

            }
            else
            {
                buffer = File.ReadAllBytes($@"{MainSettings.MainFolder}\index.html");
                response.ContentType = "text/html";
            }
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
        static void GetContextCallback(IAsyncResult ar)
        {
            try
            {
                var context = Listener.EndGetContext(ar);
                Listener.BeginGetContext(GetContextCallback, null);
                var NowTime = DateTime.UtcNow;
                
                    
                    Logs.Log.WriteWeb(Logs.TypeLog.Message, $"Запрос с {context.Request.Url.OriginalString}");

             
                

                OnNewRespone(context);

            }
            catch (Exception ex)
            {

                Logs.Log.WriteWeb(Logs.TypeLog.Error, ex.Message);

            }
           

        }

    
    }
}
