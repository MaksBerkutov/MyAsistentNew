using System;
//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        private class TelegramBotModule: IHttpsRespones
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

            public void StartHandle() => OnUpdateSettings?.Invoke("TelegramBotModule");

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<TelegramBotModule>(Date);
               
                MainSettings.APIKey = this.api = item.api;

                MainSettings.StatusTBot = this.check = item.check;
                
                if (id != item.id && long.TryParse(item.id, out var Tid))
                {
                    id = item.id;
                     MainSettings.WhiteListID.Add(Tid);
                   
                }



            }
        }

    
    }
}
