using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace MyAssistentDLL.Module.Internet
{


    internal partial class InternetWorkerModule
    {
        internal static class DispetcherClassesConection
        {
            private static Timer ClecarCloseConnection;
      

            private static void ClearCloseConnectionCallback(object state)
            {
                CheckCloseConnection();
            }

           
            public static void Start()
            {
                if (ClecarCloseConnection != null)
                {
                    Stop();
                }

                ClecarCloseConnection = new Timer(ClearCloseConnectionCallback, null, 0, (int)MainSettings.CheckConnectServer.TotalMilliseconds);
            }
            public static void Stop()
            {
                ClecarCloseConnection?.Change(Timeout.Infinite, Timeout.Infinite);
                ClecarCloseConnection = null; ClecarCloseConnection = null;
            }



            public static List<ArduinoClient> ArduinoClients = new List<ArduinoClient>();
            public static List<PCClient> PCClients = new List<PCClient>();
            public static List<IInternetClient> ALLDevice{ get {
                    var ret = new List<IInternetClient>(ArduinoClients);
                    ret.AddRange(PCClients);
                    return ret;
                } 
            }
            private static void CheckCloseConnection()
            {
                var finded = ALLDevice.FindAll(p => !p.Connect);
                foreach(var client in finded)
                    CloseConnect = client; 
            }
            public static IInternetClient CloseConnect
            {
                set
                {

                    if (ArduinoClients.ToList().Find(x => x == value) != null)
                    {
                        Logs.Log.Write(Logs.TypeLog.Message, $"Плата закрыла соеденение Имя: {value.Name}, {value.Ip} !");
                        ArduinoClients.Remove((ArduinoClient)value);
                    }
                    else if (PCClients.ToList().Find(x => x == value) != null)
                    {
                        PCClients.Remove((PCClient)value);
                    }
                }
            }
        }

    }
}
