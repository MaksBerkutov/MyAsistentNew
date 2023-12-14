using Mono.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        public delegate void UpdateSettings(string Action);

        private static event UpdateSettings OnUpdateSettings;
        public static UpdateSettings EventUpdateSettings => OnUpdateSettings;
        public interface IHttpsRespones
        {
            void Update(string Date);
            void StartHandle();
            string Get();
        }


        internal static List<IHttpsRespones> ListClasses;
        //End
        internal static HttpListener Listener = null;
        internal static readonly Dictionary<string, string> ConetneType = new Dictionary<string, string>
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

        internal static void Close()
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
        internal static void Init()
        {
            ListClasses  = new List<IHttpsRespones>()
        {

            new WebServerSettings(),
            new MainSettingsModule(),
            new TelegramBotModule(),
            new InjectionModule(),
            new ServerSettings()
        };
        }
        internal static async void Start()
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
        private static void OnNewRespone(HttpListenerContext context)
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
        private static void GetContextCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext context = Listener.EndGetContext(ar);
                Listener.BeginGetContext(GetContextCallback, null);

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
