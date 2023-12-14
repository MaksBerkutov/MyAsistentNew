using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//netsh http add iplisten 192.168.1.200
namespace MyAssistentDLL.Module.Internet.WebServer
{
    public static partial class WebServerManager
    {
        private class ServerSettings: IHttpsRespones
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

            public void StartHandle() => OnUpdateSettings?.Invoke("ServerSettings");

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

    
    }
}
