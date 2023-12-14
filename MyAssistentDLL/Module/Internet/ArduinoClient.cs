using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MyAssistentDLL.Module.Internet
{
    internal class ArduinoClient : InternetClient, IInternetClient
    {
        public bool UnCorrectPlate => !OffTherads;
        string ip;
        public string Ip => ip;
        Socket socket;
        Timer waiter;
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
        private void WaiterCallback(object state)
        {
            OffTherads = false;
            Logs.Log.Write(Logs.TypeLog.Error, "Плата не была инициализирована!(A0111)");

            waiter.Change(Timeout.Infinite, Timeout.Infinite);
        }
        void setup()
        {
            socket.Send(Encoding.ASCII.GetBytes(MainArduinoCommand.command[0]));
            waiter = new Timer(WaiterCallback, null, 0, (int)MainSettings.WaitForConectionDevice.TotalMilliseconds);

            while (OffTherads)
                while (socket.Available > 0)
                {
                    var ByteArrayeForRead = new Byte[socket.Available];
                    var message = socket.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                    UploadDate(Encoding.ASCII.GetString(ByteArrayeForRead));
                    Logs.Log.Write(Logs.TypeLog.Message, $"Подключенно устройсвто Ардуино Имя: {name}, {Ip} !");
                    waiter.Change(Timeout.Infinite, Timeout.Infinite);
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
            DateTime offtime = DateTime.Now.AddSeconds(30);
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
        public  string OTAUpdate(string Path)
        {

            try
            {
                FileStream fs = new FileStream(Path, FileMode.Open);
                string ipAddress = ip.Split(':')[0];
                int port = 8266;
                Uploader uploader = new Uploader();
                this.Send("OTA");
                uploader.FirmwareUpload(ipAddress, port,name , fs);
                return(uploader.Log);

            }
            catch (Exception ex )
            {

                Console.WriteLine(ex.Message);
                return ex.Message;
            }
           
            
        }
        public override string ToString() => $"{this.name}|({this.ip})";
    }
}
