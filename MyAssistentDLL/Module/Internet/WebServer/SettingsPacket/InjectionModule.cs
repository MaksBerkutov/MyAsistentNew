using System;
using System.Linq;
//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        private class InjectionModule : IHttpsRespones
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

            public void StartHandle() => OnUpdateSettings?.Invoke("InjectionModule");

            public void Update(string Date)
            {
                var item = Newtonsoft.Json.JsonConvert.DeserializeObject<InjectionModule>(Date);
                MainSettings.StatusInject = this.check = item.check;
                if (login != item.login && item.login.Any() && item.pass.Any()) 
                {
                    login = item.login;
                    Account.Add(item.login, item.pass);

                }



            }
        }

    
    }
}
