using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace MyAsistent.Module.Internet.CodeInject
{
    public class PacketDevice
    {
        public string PublicKey { get; set; }
        public string Date { get; set; }

        private static PacketDevice CreateFromRegex(MatchCollection matches)
        {
            return new PacketDevice() { PublicKey = matches[0].Groups[1].Value, Date = matches[1].Groups[1].Value };
        }

        public static PacketDevice ConvertFromString(string input)
        {
            string pattern = @"\[(.*?)\]";
            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches.Count == 2)
                return CreateFromRegex(matches);

            return null;
        }

        public override string ToString() => $"[{this.PublicKey}][{this.Date}]";
    }

    public class ObjectDevice
    {
        public string Command;
        public List<string> Args;

        public ObjectDevice(List<string> Input)
        {
            if (Input == null || Input.Count == 0)
            {
                throw new ArgumentException("Input list is null or empty.");
            }

            Command = Input.FirstOrDefault();
            Args = Input.Skip(1).ToList();
        }
    }

    public class Device
    {
        public string SessionLogin { get; set; }

        private Socket Connection;

        private string PublicKey;
        private string PrivateKey;

        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        

        private bool CheckConnected()
        {
            bool part1 = Connection.Poll(1000, SelectMode.SelectRead);
            bool part2 = (Connection.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        public bool isConnected()
        {
            try
            {
                return CheckConnected();
            }
            catch (Exception)
            {

                return false;
            }

        }

        public void CloseDevicesConnection()
        {
            Account.CloseConnection(SessionLogin);
            Connection.Dispose();
        }

        private void SendPacket(string Date)
        {
            Date = Date == string.Empty ? string.Empty : EncryptWithPublicKey(Date);
            Connection.Send(Encoding.UTF8.GetBytes(
             new PacketDevice() { Date = Date, PublicKey = GenerateKeys() }
             .ToString()));
        }

        public Device(Socket Connection)
        {
            this.Connection = Connection;

            this.Connection.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, this.Connection);

            this.SendPacket(string.Empty);
        }

        private PacketDevice Recive(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received = current.EndReceive(AR);
            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            var readString = Encoding.UTF8.GetString(recBuf);
            return PacketDevice.ConvertFromString(readString);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                PacketDevice ReciveDate = Recive(AR);
                ConvertDatePackets(ReciveDate);
                if (Connection != null)
                    Connection.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, Connection);
            }
            catch (SocketException ex)
            {
                //Add Log Function
                Connection.Close();
            }
            
        }

        private string GenerateKeys()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {

                PrivateKey = rsa.ToXmlString(true);
                return rsa.ToXmlString(false);
            }
        }
        private string EncryptWithPublicKey(string plainText)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PublicKey);
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = rsa.Encrypt(data, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
        private string DecryptWithPrivateKey(string encryptedText)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        private void ConvertDatePackets(PacketDevice InputPacket)
        {
            if (InputPacket != null && InputPacket.PublicKey.Any())
                this.PublicKey = InputPacket.PublicKey;
            else return;
            if (InputPacket.Date.Any())
            {
                var decryptedData = DecryptWithPrivateKey(InputPacket.Date);
                ObjectDevice deserelizeData = new ObjectDevice(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(decryptedData));
                HandlerCommand(deserelizeData);
            }

        }

        private void Authentication(Account Arg)
        {
            if (Arg == null || Arg.Connection)
            {
                this.SendPacket("false");
                this.Connection.Dispose();
                Connection = null;
                return;
            }
            SessionLogin = Arg.Login;
            this.SendPacket(Arg.Login);
        }
        private void Authentication(List<string> Arg)
        {
            if(Arg.Count == 2)
                Authentication(Account.FindUser(Arg[0], Arg[1]));
                /*
                0 index = Login
                1 index = Password
                */
        }

        private void HandlerCommand(ObjectDevice Obj)
        {
            switch (Obj.Command)
            {
                case "Authentication":
                    Authentication(Obj.Args);
                    break;
                case "Voice":
                    Sound.Voice.PlayRuAsync(Obj.Args[0]);
                    break;
                case "Arduino":
                    InternetWorkerModule.SendToClent(Obj.Args[0], Obj.Args[1]);
                    break;
                case "ArduinoDate":
                    if (InternetWorkerModule.SendToClinetRecive(Obj.Args[0], Obj.Args[1], out var res))
                        this.SendPacket(res);
                    else
                        this.SendPacket("NULL");
                    break;


            }
        }

       


    }

    public static class Injection
    {
        private static Socket InjectConnection;
        private static List<Device> Devices = new List<Device>();
        private static DispatcherTimer KillTimer = new DispatcherTimer();

        public static void CloseServer()
        {
            if (KillTimer == null) return;
            KillTimer.Stop();
            KillTimer = null;
            if (InjectConnection != null)
            {
                InjectConnection.Dispose();
                InjectConnection.Close();

                InjectConnection = null;
            }


            var tmp = new List<Device>(Devices.ToArray());
            foreach (var item in tmp)
            {
                item.CloseDevicesConnection();
                Devices.Remove(item);
            }
        }

        public static void AcceptCallbackDevices(IAsyncResult ar)
        {
            try
            {
                Socket handler = InjectConnection.EndAccept(ar);
                Devices.Add(new Device(handler));
                InjectConnection.BeginAccept(new AsyncCallback(AcceptCallbackDevices), null);


            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }
       

        }

        private static void CreateDefaultSocket()
        {
            IPAddress ipAddress = IPAddress.Parse(MainSettings.IpInject);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, MainSettings.PortInject);
            InjectConnection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            InjectConnection.Bind(localEndPoint);
            InjectConnection.Listen(0);
            InjectConnection.BeginAccept(new AsyncCallback(AcceptCallbackDevices), null);
          

        }

        private static void KillTimer_Tick(object sender, EventArgs e)
        {
            var tmp = new List<Device>(Devices.ToArray());
            foreach (var item in tmp)
            {
                if (!item.isConnected())
                {
                    Logs.Log.Write(Logs.TypeLog.Message, $"InjectWorker session closed about [{item.SessionLogin}]");
                    item.CloseDevicesConnection();
                    Devices.Remove(item);
                }
            }
        }

        private static void InitializationKillerTime()
        {
            if (KillTimer == null) KillTimer = new DispatcherTimer();
            KillTimer.Interval = MainSettings.CheckConnectServer;
            KillTimer.Tick += KillTimer_Tick;
            KillTimer.Start();
        }

        public static void StartServer()
        {
            try
            {
                CreateDefaultSocket();
                InitializationKillerTime();
                Logs.Log.Write(Logs.TypeLog.Message, $"Inject stared at {InjectConnection.LocalEndPoint}!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Logs.Log.Write(Logs.TypeLog.Error, e.Message);
            }
        }



    }
}
