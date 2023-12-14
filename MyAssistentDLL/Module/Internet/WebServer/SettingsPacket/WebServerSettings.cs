//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        //Classe to send 
        private class WebServerSettings : IHttpsRespones {
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

            public void StartHandle() => OnUpdateSettings?.Invoke("WebServerSettings");

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<WebServerSettings>(Date);
                MainSettings.StatusWebServer = this.check = item.check;
                MainSettings.MainFolder = this.path = item.path;
                MainSettings.MainPrefix = this.Url = item.Url;
            }
        }

    
    }
}
