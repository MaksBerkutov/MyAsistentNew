using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Net;

namespace MyAsistent.Module.Internet.CodeInject
{
    class Pakage
    {
        public string Key { get; set; }
        public string Reqest { get; set; }
        public string Resume { get; set; }
    }

    public static  class InjectWorker
    {
        private static Dictionary <string , ItemInjection > GeneretedKeySesion = new Dictionary<string, ItemInjection>();

        private static Random random = new Random();
        private readonly static int KeyLenght = 8;
        public static string GenerateKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZqwertyuiopasdfghjklmnbvcxz0123456789";
            return new string(Enumerable.Repeat(chars, KeyLenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        class ItemInjection
        {
            public string KeySesion { get; set; }
            public string LoginOfSession { get; set; }
            public void Close() => Account.CloseConnection(LoginOfSession);
            private  Socket Conn;
            private string MyKey;
            private const int BUFFER_SIZE = 2048;
            private static readonly byte[] buffer = new byte[BUFFER_SIZE];
            public bool SocketConnected()
            {
                try
                {
                    bool part1 = Conn.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (Conn.Available == 0);
                    if (part1 && part2)
                        return false;
                    else
                        return true;
                }
                catch (Exception)
                {

                    return false;
                }
               
            }

            public delegate void delKil(ItemInjection obj);
            DispatcherTimer KillerTime;
            public event delKil KillMe;
            public ItemInjection(Socket Conn)
            {
                this.Conn = Conn;
               

                this.Conn.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, this.Conn);


            }
            private  void ReceiveCallback(IAsyncResult AR)
            {
                Socket current = (Socket)AR.AsyncState;
                int received;

                try
                {
                    received = current.EndReceive(AR);
                }
                catch (SocketException)
                {
                    current.Close();
                    
                    return;
                }

                byte[] recBuf = new byte[received];
                Array.Copy(buffer, recBuf, received);
                var strJson = Encoding.UTF8.GetString(recBuf);
                if (strJson.Split('{').Length > 2)
                {
                    MultiplateWorker(strJson);return;
                }
                var pak = Newtonsoft.Json.JsonConvert.DeserializeObject<Pakage>(strJson);
                var Decode = GetInfo(pak.Reqest);
                if (Decode.Key == "Auth")
                {
                    AuthModule(Decode, pak);
                }
                else
                {
                    Worker(Decode, pak);
                }

               if(Conn!=null)   
                 current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
            }
            private void MultiplateWorker(string strMulti)
            {
                var AllCmd = strMulti.Split('{');
                foreach(var strJson in AllCmd)
                {
                    if (strJson.Length == 0) continue;
                    var pak = Newtonsoft.Json.JsonConvert.DeserializeObject<Pakage>($"{{{strJson}");
                    var Decode = GetInfo(pak.Reqest);
                    if (Decode.Key == "Auth")
                    {
                        if (!AuthModule(Decode, pak)) return ;
                    }
                    else
                    {
                        Worker(Decode, pak);
                    }
                }
            }
            private static  (string Key, string[] Arg) GetInfo(string req)
            {
                return(req.Split('=')[0],req.Split('=')[1].Split(','));
            }
            private  bool AuthModule((string Key, string[] Arg) Obj,Pakage pkj)
            {
                var finded = Account.FindUser(Obj.Arg[0], Obj.Arg[1]);
                if (finded == null||finded.Connection) { pkj.Reqest = "false"; Send(pkj); Conn.Close();Conn.Dispose(); Conn = null; return false; }
                while (true)
                {
                   
                    var key = GenerateKey();
                    if (GeneretedKeySesion.TryGetValue(key, out var val)) { continue; }
                    GeneretedKeySesion.Add(key, this);
                    MyKey = pkj.Reqest = key; Send(pkj);
                    LoginOfSession = Obj.Arg[0];
                    finded.OpenCoonnection();
                    return true;
                }

            }
            private void Worker((string Key, string[] Arg) Obj, Pakage pkj)
            {
                switch (Obj.Key)
                {
                    case "Voice":
                        Sound.Voice.PlayRuAsync(Obj.Arg[0]);
                        break;
                    case "Arduino":
                        InternetWorkerModule.SendToClent(Obj.Arg[0], Obj.Arg[1]);break;
                    case "ArduinoDate":
                        if(InternetWorkerModule.SendToClinetRecive(Obj.Arg[0], Obj.Arg[1],out var res))
                        {
                            pkj.Resume = res;Send(pkj); return;
                        }
                        else
                        {
                            pkj.Resume = "NULL"; Send(pkj); return;
                        }
                       
                }
            }
      
            private void Send(Pakage pk)=>Conn.Send(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(pk)));
           

        }
        private static Socket Inject;
        private static DispatcherTimer KillTimer = new DispatcherTimer();
        private static List<ItemInjection> InjectionsList = new List<ItemInjection>();
        public static void INIT()
        {
            IPHostEntry host = Dns.GetHostEntry(MainSettings.IpInject);
            IPAddress ipAddress = host.AddressList[MainSettings.AddresListIDServer];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, MainSettings.PortInject);
            try
            {
                Inject = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Inject.Bind(localEndPoint);
                Inject.Listen(10);
                Inject.BeginAccept(
                new AsyncCallback(AcceptCallbackUsers), null);
                if(KillTimer==null) KillTimer = new DispatcherTimer();
                KillTimer.Interval = MainSettings.CheckConnectServer;
                KillTimer.Tick += KillTimer_Tick;
                KillTimer.Start();
                Logs.Log.Write(Logs.TypeLog.Message, $"Inject stared at {Inject.LocalEndPoint}!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Logs.Log.Write(Logs.TypeLog.Error, e.Message);
            }
        }
        public static void OffInject()
        {
            if (KillTimer == null) return;
            KillTimer.Stop();
            KillTimer = null;
           if(Inject != null)
            {
                Inject.Dispose();
                Inject.Close();

                Inject = null;
            }
           

            var tmp = new List<ItemInjection>(InjectionsList.ToArray());
            foreach (var item in tmp)
            {
                item.Close();
                InjectionsList.Remove(item);
            }
        }
        private static void KillTimer_Tick(object sender, EventArgs e)
        {
           var tmp = new List<ItemInjection>(InjectionsList.ToArray());
           foreach(var item in tmp)
            {
                if (!item.SocketConnected())
                {
                    Logs.Log.Write(Logs.TypeLog.Message, $"InjectWorker session closed about [{item.LoginOfSession}]");
                    item.Close();  
                    InjectionsList.Remove(item);
                }
            } 
        }

        public static void AcceptCallbackUsers(IAsyncResult ar)
        {
            try
            {
                Socket handler = Inject.EndAccept(ar);
                InjectionsList.Add(new ItemInjection(handler));
                Inject.BeginAccept(new AsyncCallback(AcceptCallbackUsers), null);
            }
            catch (Exception ex )
            {
              Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }
           
        }

    }
}
