using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Connector
{
    public class Connector
    {
        private string _ip = "127.0.0.1"; private int _port = 2745;
        private string _login = null, _password = null;
      
        public Connector(string login, string passworld,string ip = null,int? port = null)
        {
            _login = login;
            _password = passworld;
            if (ip != null)
            {
                _ip = ip;_port = port.Value;
            }

        }
      
        public Conection GetConnection()
        {
            try
            {

                IPHostEntry ipHost = Dns.GetHostEntry(_ip);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, _port);

                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(localEndPoint);
                    byte[] messageSent = Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new Worker.Worker.Pakage { Reqest = $"Auth={_login},{_password}" }));
                    int byteSent = sender.Send(messageSent);
                    while (true)
                        while (sender.Available > 0)
                        {
                            byte[] messageReceived = new byte[sender.Available];
                            sender.Receive(messageReceived);
                            var Result = Newtonsoft.Json.JsonConvert.DeserializeObject<Worker.Worker.Pakage>(Encoding.ASCII.GetString(messageReceived));
                            if (Result.Resume == "false") return null;
                            else return new Conection(sender, Result.Resume);

                        }

                   
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            return null;

        }
        public async Task<Conection> GetConectionAsync()
        {
            return await Task.Run(() => GetConnection());
        }
    }

    public class Conection
    {
        private Socket conn;
        private string AuthKey;
        public delegate void NewPakaje(Worker.Worker.Pakage pak);
        public delegate void NewMess(string pak);
        public event NewPakaje OnNewPakaje;
        public event NewMess OnNewMsg;
        public Conection(Socket conn, string authKey)
        {
            this.conn = conn;
            AuthKey = authKey;
        }
        void Read()
        {
            while (true)
                while (conn.Available > 0)
                {
                    byte[] read = new byte[conn.Available];
                    conn.Receive(read);
                    string result = Encoding.UTF8.GetString(read);
                    try
                    {
                        var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Worker.Worker.Pakage>(result);
                        OnNewPakaje?.Invoke(ret);continue;
                    }
                    catch (Exception)
                    {

                        
                    }
                    OnNewMsg?.Invoke(result);

                }
        }
        public int Receive(byte[]arr)=>conn.Receive(arr);
        public int Available => conn.Available;
        public Worker.Worker GetWorker()=>new Worker.Worker(this);
        public void SignaturePakage(ref Worker.Worker.Pakage obj) => obj.Key = AuthKey;
        public void SendPackaje(string json) => conn.Send(Encoding.UTF8.GetBytes (json));
    }
    namespace Worker
    {
        public class Worker
        {
           public class AsistenetVariableManager
            {
                List<AssistentVariable> items;

                public AsistenetVariableManager()
                {
                    items = new List<AssistentVariable>();
                }

                public AssistentVariable this [int i]=>items[i];
                public AssistentVariable this [string i]=>items.Find(x=>x.Name==i);
                public class AssistentVariable
                {
                    string name;
                    string value;

                    public AssistentVariable(string name, string value)
                    {
                        this.name = name;
                        this.value = value;
                    }

                    public string Name => name;
                    public string Value => value;

                }
                public void add(AssistentVariable obj)=>items.Add(obj);
            }
            private List<string> sendsCmd = new List<string>()
            {
                "SendArduino",
                "GetArduino",
                "Voice",
            };
            public class Pakage
            {
                public string Key { get; set; }
                public string Reqest { get; set; }
                public string Resume { get; set; }
            }
            Conection conn;
            public Worker(Conection conn)=>this.conn = conn;
            //User Function
            public void Voice(string VoiceToSpech) => SendPacakge(new Pakage { Reqest = $"Voice={VoiceToSpech}" });
            public void SendArduino(string NamePlate,string Command) => SendPacakge(new Pakage { Reqest = $"Arduino={NamePlate},{Command}" });
            public AsistenetVariableManager SendArduinoRecive(string NamePlate, string Command)
            {
                var item = SendgetResult(new Pakage { Reqest = $"ArduinoDate={NamePlate},{Command}" }).Split('_');
                AsistenetVariableManager obj = new AsistenetVariableManager();
                foreach (var i in item)
                {
                    var name = i.Split(':')[0];
                    name = name.Remove(0,1);
                    name = name.Remove(name.Length-1,1);
                    obj.add(new AsistenetVariableManager.AssistentVariable(name, i.Split(':')[1]));
                }return obj;
            }

            private string SendgetResult(Pakage pak) {
                SendPacakge(pak);
                while (true)
                    while (conn.Available > 0)
                    {
                        byte[] read = new byte[conn.Available];
                        conn.Receive(read);
                        try
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<Pakage>(
                                Encoding.UTF8.GetString(read)).Resume;
                        }
                        catch (Exception)
                        {
                            return "null";

                        }
                    }

            }
            private async Task<string> SendgetResultAsync(Pakage pak)
            {
                return await Task.Run(() => SendgetResult(pak));
            }
            private void SendPacakge(Pakage pak)
            {
                conn.SignaturePakage(ref pak);
                conn.SendPackaje(Newtonsoft.Json.JsonConvert.SerializeObject(pak));
            }
            private async void SendPacakgeAsync(Pakage pak)
            {
                await Task.Run(()=> SendPacakge(pak));
            } 

        }
    }
}


