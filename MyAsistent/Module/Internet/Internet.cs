using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MyAsistent.Module.Internet
{
    //Error Module ID 011
   class MainArduinoCommand
   {
      public static readonly List<string> command  = new List<string>() { 
          "SERV_GAI",//Get ALL Info NOTE: From Arduino WARNING: 0 index this is name arduino in server
          "SERV_GP"//GET platform
      };
      public static readonly Dictionary<string, string> InfoAboutCommand = new Dictionary<string, string>()
      {
          {command[0],"Получение краткой информации об устройстве которое пытаеться установить соедение с сервером.\n" +
              "Ситаксис ответа: \"NamePlate.DiscriptionPlate\"" },
          {command[1],"Получение информации о устройстве которое пытаеться установить подключение\n" +
              "Это нужно для того что бы сервер понимал какеи метод применять." }
      };
   }
    public interface IInternetClient
    {
       
        string Ip { get; }
        Socket Sockets { get; }
        string Name { get; }
        bool Connect { get; }
        bool UnCorrectPlate{ get; }
        void Send(string msg);
        
        string SendAndRecive(string msg);
       
        string ToString();
    }
    public abstract class InternetClient{
        
        public InternetClient(Socket socket)
        {

        }
     


    }
    public class ArduinoClient : InternetClient, IInternetClient
    {
        public bool UnCorrectPlate => !OffTherads;
        string ip;
        public string Ip => ip;
        Socket socket;
        DispatcherTimer waiter;
        bool OffTherads = true;
        public Socket Sockets => socket;
        private string name;
        public string Name => name;
        ObservableCollection<string> command;
        public ObservableCollection<string> Command => command;
        public ObservableCollection<string> AllCommand {
            get
            {
                var tmp = command.ToList();
                tmp.AddRange(commandRec.ToList());
                return new ObservableCollection<string>(tmp.ToArray());
            }
        }
        ObservableCollection<string> commandRec;
        public ObservableCollection<string> CommandRec => commandRec;
        public ArduinoClient(Socket socket):base(socket)
        {
            try
            {
                this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
                ip = socket.RemoteEndPoint.ToString();
                setup();
               
            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }
        }
        void UploadDate(string ReadDate)
        {
            
            var ReadArray = ReadDate.Split('.');
            name= ReadArray[0];
            command = new ObservableCollection<string>();
            commandRec = new ObservableCollection<string>();
            for(int i = 1; i < ReadArray.Length; i++)
            {
                var TmpArr = ReadArray[i].Split('_');
                if(TmpArr.Length == 0)
                {
                    command.Add(ReadArray[i]);
                }
                if(TmpArr[TmpArr.Length-1]== "REC") commandRec.Add(ReadArray[i]);
                else command.Add(ReadArray[i]);
            }
        }
        void setup()
        {
            socket.Send(Encoding.ASCII.GetBytes(MainArduinoCommand.command[0]));
            waiter = new DispatcherTimer();
            waiter.Interval = MainSettings.WaitForConectionDevice;
            waiter.Tick += (object sender, EventArgs e) =>
        {
            OffTherads = false;
            Logs.Log.Write(Logs.TypeLog.Error, "Плата не была иницилизирована!(A0111)");
            ((DispatcherTimer)sender).Stop();
        };
            waiter.Start();
            while(OffTherads)
                while (socket.Available > 0)
                {
                    var ByteArrayeForRead = new Byte[socket.Available];
                    var message = socket.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                    UploadDate(Encoding.ASCII.GetString(ByteArrayeForRead));
                    //command = new ObservableCollection<string>(Encoding.ASCII.GetString(ByteArrayeForRead).Split('.').ToList().ToArray());
                    //name = command[0];command.RemoveAt(0);
                    Logs.Log.Write(Logs.TypeLog.Message, $"Подключенно устройсвто Ардуино Имя: {name}, {Ip} !");
                    waiter.Stop();
                    return;
                }
            

        }
       
        public void Send(string msg)=> socket.Send(Encoding.ASCII.GetBytes(msg));
        public string SendAndRecive(string msg)
        {
            socket.Send(Encoding.ASCII.GetBytes(msg));
            return Read();
        }
        string Read()
        {
            DateTime offtime = DateTime.Now.AddSeconds(5);
            while (OffTherads)
                if (offtime < DateTime.Now)
                    while (socket.Available > 0)
                    {
                        var ByteArrayeForRead = new Byte[socket.Available];
                        var message = socket.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                        return Encoding.ASCII.GetString(ByteArrayeForRead);
                    }
                else return "Plate No Get Result!";
            return null;    
        }
        bool SocketConnected()
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        public bool Connect => SocketConnected();
        public string OTAUpdate(byte[] bin)
        {
            socket.Send(Encoding.ASCII.GetBytes("OTA"));
            socket.Send(bin);
            return Read();
        }


        public override string ToString() => $"{this.name}|({this.ip})";
    }
    public class PCClient : InternetClient, IInternetClient
    {
        private class SendPak
        {
            public int id;
            public string Input_Cmd;
            public bool Out;public string Req;
        }
        //comand Get_NameParametrOfSettings, GETallValue_NameValue; [input]
       
        private string ip;
        public string Ip => ip;

        private Socket socket;
        public Socket Sockets => socket;

        private string name;
        public string Name => name;
        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        public bool Connect => false;
        bool OffTherads = true;
        public bool UnCorrectPlate => !OffTherads;
        private async void Handler(SendPak Input)
        {
            await Task.Run(() =>
            {

            });
        }
        private void ReceiveCallback(IAsyncResult AR)
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
            Console.WriteLine(strJson);
            
            if (socket != null)
                current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
        public PCClient(Socket socket) : base(socket)
        {
            try
            {
                this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
                this.socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, this.socket);
                ip = socket.RemoteEndPoint.ToString();
            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }
        }
        public void Send(string msg) => socket.Send(Encoding.ASCII.GetBytes(msg));
        public string SendAndRecive(string msg) => null;
        string Read() => null;
    }
    public class UniversalClient : InternetClient, IInternetClient
    {


        private string ip;
        public string Ip => ip;

        private Socket socket;
        public Socket Sockets => socket;

        private string name;
        public string Name => name;

        public bool Connect => false;
        bool OffTherads = true;
        public bool UnCorrectPlate => !OffTherads;

        public UniversalClient(Socket socket) : base(socket)
        {
            try
            {
                this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
                ip = socket.RemoteEndPoint.ToString();
            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }
        }
        public void Send(string msg) => socket.Send(Encoding.ASCII.GetBytes(msg));
        public string SendAndRecive(string msg)
        {
            socket.Send(Encoding.ASCII.GetBytes(msg));
            return Read();
        }
        string Read()
        {
            while (OffTherads)
                while (socket.Available > 0)
                {
                    var ByteArrayeForRead = new Byte[socket.Available];
                    socket.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                    return Encoding.ASCII.GetString(ByteArrayeForRead);
                }
            return null;
        }
    }


    public class InternetWorkerModule
    {
        static Socket listener;
        public static class DispetcherClassesConection
        {
            private static DispatcherTimer ClecarCloseConnection;
            public static void Start()
            {
                if (ClecarCloseConnection != null)Stop();
                ClecarCloseConnection = new DispatcherTimer();
                ClecarCloseConnection.Tick += (object sender, EventArgs e)=>
                {
                    CheckCloseConnection();
                };
                ClecarCloseConnection.Interval = MainSettings.CheckConnectServer;
                ClecarCloseConnection.Start();
            }
            public static void Stop()
            {
                ClecarCloseConnection.Stop(); ClecarCloseConnection = null;
            }



            public static ObservableCollection<ArduinoClient> ArduinoClients = new ObservableCollection<ArduinoClient>();
            public static ObservableCollection<PCClient> PCClients = new ObservableCollection<PCClient>();
            public static ObservableCollection<UniversalClient> UCClients = new ObservableCollection<UniversalClient>();
            public static List<IInternetClient> ALLDevice{ get {
                    var ret = new List<IInternetClient>(ArduinoClients);
                    ret.AddRange(PCClients);
                    ret.AddRange(UCClients);
                    return ret;
                } 
            }
            private static void CheckCloseConnection()
            {
                var finded = ALLDevice.FindAll(p => !p.Connect);
                foreach(var client in finded)
                    CloseConnect = client; 
            }      
            public static IInternetClient CloseConnect { set {
                    MainWindow.dispatcher.Invoke(new Action(() =>
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
                        else if (UCClients.ToList().Find(x => x == value) != null)
                        {
                            UCClients.Remove((UniversalClient)value);
                        }




                    }));
                  
                } }
        }
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
       
        private class ConnectorHelper
        {
            bool IsRuning = true;
            private string Getplatform(Socket obj)
            {
                obj.Send(Encoding.ASCII.GetBytes(MainArduinoCommand.command[1]));
                while (IsRuning)
                    while (obj.Available > 0)
                    {
                        var ByteArrayeForRead = new Byte[obj.Available];
                        obj.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                        return Encoding.ASCII.GetString(ByteArrayeForRead);

                    }
                return "Error";
            }
            public ConnectorHelper(Socket client)
            {
                Start(client);
            }
            private async void Start(Socket client)
            {
                await Task.Run(() =>
                {
                    DispatcherTimer dm = new DispatcherTimer();
                    dm.Interval = MainSettings.WaitForConectionDevice;
                    dm.Tick += (sender, e) => {
                        IsRuning = false;
                        ((DispatcherTimer)sender).Stop();
                    };
                    dm.Start();
                    string Platform = Getplatform(client);
                    mutex.WaitOne();

                    MainWindow.dispatcher.Invoke(new Action(() =>
                    {
                        if (Platform == "Arduino")
                            DispetcherClassesConection.ArduinoClients.Add(new ArduinoClient(client));
                        else if (Platform == "PC")
                            DispetcherClassesConection.PCClients.Add(new PCClient(client));
                        else if (Platform == "Error")
                            Logs.Log.Write(Logs.TypeLog.Error, "Плата не была иницилизирована!(A0112)");
                        

                    }));
                    mutex.ReleaseMutex();
                    dm.Stop();
                });
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



















        //private static string GetPlatform(Socket obj)
        //{


        //    DispatcherTimer dm = new DispatcherTimer();
        //    dm.Interval = MainSettings.WaitForConectionDevice;
        //    bool IsRunning = true;
        //    dm.Tick += (sender, e) => { };
        //    dm.Start();
        //    obj.Send(Encoding.ASCII.GetBytes(MainArduinoCommand.command[1]));
        //    while (true)
        //        while (obj.Available > 0)
        //        {
        //            var ByteArrayeForRead = new Byte[obj.Available];
        //            obj.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
        //            return Encoding.ASCII.GetString(ByteArrayeForRead);

        //        }
        //   return "Error";
        //}


        //private static async void ConnectionHandler(Socket client)
        //{
        //    await Task.Run(() =>
        //    {
        //        string Platform = GetPlatform(client);
        //        mutex.WaitOne();

        //        MainWindow.dispatcher.Invoke(new Action(() =>
        //        {
        //            if (Platform == "Arduino")
        //                DisoetcherClassesConection.Users.Add(new ArduinoClient(client));
        //            else if (Platform == "Error")
        //                Logs.Log.Write(Logs.TypeLog.Error, "Плата не была иницилизирована!(A0112)");
        //            //else if (Platform == "Android")
        //            //Android.Add(new AndroidClinet(client));

        //        }));
        //        mutex.ReleaseMutex();
        //    });
        //}

    }
}


























































































//public class AndroidClinet : InternetClient, IInternetClient
//{
//    class DateSend
//    {
//        string save;
//        List<string> voice;
//        List<string> sound;
//        List<string> com;
//        public DateSend()
//        {
//            save = Newtonsoft.Json.JsonConvert.SerializeObject(new MainSettings.Saves());
//            voice = new List<string>();
//            sound = new List<string>();
//            com = Codes.COMSENDER.GetAllCom().ToList();
//            foreach (var i in MyAsistent.Module.Sound.Voice.GetName())
//                voice.Add(i.ToString());
//            foreach (var i in MyAsistent.Module.Sound.Sound.getAllCultures())
//                if (i.Culture.Name.Length != 0)
//                    sound.Add(i.Culture.Name);
//        }
//        public override string ToString()=> Newtonsoft.Json.JsonConvert.SerializeObject(this);

//    }
//    string ip;
//    public string Ip => ip;
//    Socket socket;
//    public Socket Sockets => socket;
//    private string name;
//    public string Name { get => name; }
//    public System.Windows.Threading.DispatcherTimer SendUpdateDate;
//    public AndroidClinet(Socket socket) : base(socket)
//    {
//        try
//        {
//            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
//            ip = socket.RemoteEndPoint.ToString();
//            name = $"ANDROID{socket.RemoteEndPoint.ToString().Split('.')[3]}";
//            setup();
//        }
//        catch (Exception ex)
//        {
//            Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
//        }
//    }
//    void setup()
//    {
//        socket.Send(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new DateSend().ToString())));
//        SendUpdateDate = new System.Windows.Threading.DispatcherTimer();
//        SendUpdateDate.Interval = new TimeSpan(0, 1, 30);
//        SendUpdateDate.Tick += new EventHandler((object sender1, EventArgs e1) => {
//            socket.Send(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new DateSend().ToString())));
//        });
//        SendUpdateDate.Start();
//    }
//    public void Send(string msg) => socket.Send(Encoding.ASCII.GetBytes(msg));
//    public string SendAndRecive(string msg)
//    {
//        socket.Send(Encoding.ASCII.GetBytes(msg));
//        return Read();
//    }
//    public string Read()
//    {
//        while (true)
//            while (socket.Available > 0)
//            {
//                var ByteArrayeForRead = new Byte[socket.Available];
//                var message = socket.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
//                return Encoding.ASCII.GetString(ByteArrayeForRead);
//            }
//    }
//    bool SocketConnected()
//    {
//        bool part1 = socket.Poll(1000, SelectMode.SelectRead);
//        bool part2 = (socket.Available == 0);
//        if (part1 && part2)
//            return false;
//        else
//            return true;
//    }
//    public bool Connect => SocketConnected();
//    public override string ToString() => $"{this.name}|({this.ip})";
//}