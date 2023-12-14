using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyAssistentDLL.Module.Internet
{


    internal partial class InternetWorkerModule
    {
        static Socket listener;
        private static Mutex mutex = new Mutex(false);
        public static void StartServer()
        {
           
            //Configure For restart
           
            if (listener != null) { listener.Close(); listener.Dispose(); }

            //
            DispetcherClassesConection.Start();
            try{
                IPHostEntry host = Dns.GetHostEntry(MainSettings.IPServer);
                IPAddress ipAddress = host.AddressList[MainSettings.AddresListIDServer];//[0]
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, MainSettings.PortServer);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                listener.BeginAccept(
                          new AsyncCallback(AcceptCallbackUsers), null);



                Logs.Log.Write(Logs.TypeLog.Message, $"Server stared at {listener.LocalEndPoint}!");
            }
            catch (SocketException ex) when (ex.ErrorCode == 10004)
            {
                return;
            }
            catch (Exception e){
                Logs.Log.Write(Logs.TypeLog.Error,e.Message);
            }

        }
       
        public static void AcceptCallbackUsers(IAsyncResult ar)
        {
           
                try
                {
                    Socket handler = listener.EndAccept(ar);

                    new ConnectorHelper(handler);
                }
                catch (Exception ex )
                {
                        Logs.Log.Write(Logs.TypeLog.Error, ex.ToString());
                }
                listener.BeginAccept(
                         new AsyncCallback(AcceptCallbackUsers), null);
        }
       
        private static IInternetClient GetClient(string Name)
        {
            if(DispetcherClassesConection.ArduinoClients.ToList().Find(p => p.Name == Name)!=null)return DispetcherClassesConection.ArduinoClients.ToList().Find(p => p.Name == Name);
            return null;
        }
        public static bool SendToClent(string ArduinoName, string ArduinoCommand)
        {
            var obj = GetClient(ArduinoName);
            if(obj == null) return false;
            obj?.Send(ArduinoCommand);return true;
        }
        public static bool SendToClinetRecive(string ArduinoName, string ArduinoCommand, out string res)
        {
            res = "";
            var obj = GetClient(ArduinoName);
            if (obj == null) return false;
            res = obj.SendAndRecive(ArduinoCommand);
            return true;
        }

    }
}
