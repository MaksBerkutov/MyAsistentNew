using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net;

namespace MyAsistent.Module.Internet.Model
{
    public interface IDevices
    {
        Socket Connection { get; }
        bool isConected { get; }

        public void CloseDevicesConnection();
        public void Start(Socket socket);

    }
    public interface IServer<T> where T : IDevices
    {
        Socket Connetions { get; }
        List<T> Devices { get; }
        public void CloseServer();
        public void StartServer();

    }

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

    public class RSAextentions
    {
        public static string GenerateKeys(ref string PrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {

                PrivateKey = rsa.ToXmlString(true);
                return rsa.ToXmlString(false);
            }
        }
        public static string EncryptWithPublicKey(string plainText, string PublicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PublicKey);
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedData = rsa.Encrypt(data, false);
                return Convert.ToBase64String(encryptedData);
            }
        }
        public static string DecryptWithPrivateKey(string encryptedText, string PrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(PrivateKey);
                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
    public abstract class Devices : IDevices
    {
        public Socket Connection { get; protected set; }
        protected string PublicKey;
        protected string PrivateKey;
        public bool isConected
        {
            get
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
        }

        private bool CheckConnected()
        {
            bool part1 = Connection.Poll(1000, SelectMode.SelectRead);
            bool part2 = (Connection.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        public Devices()
        {
        }

        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        protected virtual void SendPacket(string Date)
        {
            Date = Date == string.Empty ? string.Empty : RSAextentions.EncryptWithPublicKey(Date, PublicKey);
            Connection.Send(Encoding.UTF8.GetBytes(
             new PacketDevice() { Date = Date, PublicKey = RSAextentions.GenerateKeys(ref PrivateKey) }
             .ToString()));
        }

        public virtual void Start(Socket socket)
        {
            this.Connection = Connection;

            this.Connection.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, this.Connection);

            this.SendPacket(string.Empty);
        }
        public virtual void CloseDevicesConnection()
        {
            Connection.Dispose();
        }

        protected void ConvertDatePackets(PacketDevice InputPacket)
        {
            if (InputPacket != null && InputPacket.PublicKey.Any())
                this.PublicKey = InputPacket.PublicKey;
            else return;
            if (InputPacket.Date.Any())
            {
                var decryptedData = RSAextentions.DecryptWithPrivateKey(InputPacket.Date, this.PrivateKey);
                ObjectDevice deserelizeData = new ObjectDevice(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(decryptedData));
                HandlerCommand(deserelizeData);
            }

        }

        protected virtual PacketDevice Recive(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received = current.EndReceive(AR);
            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            var readString = Encoding.UTF8.GetString(recBuf);
            return PacketDevice.ConvertFromString(readString);
        }

        protected virtual void ReceiveCallback(IAsyncResult AR)
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
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
                Connection.Close();
            }

        }
        protected abstract void HandlerCommand(ObjectDevice Obj);

    }
    public abstract class Server<T> where T : Devices, new()
    {
        protected string Ip = string.Empty;
        protected int Port = -1;

        private Socket Connetion;
        public Socket Connetions => Connetion;
        public List<T> Devices = new List<T>();
        private DispatcherTimer KillTimer = new DispatcherTimer();

        public virtual void CloseServer()
        {
            if (KillTimer == null) return;
            KillTimer.Stop();
            KillTimer = null;
            if (Connetion != null)
            {
                Connetion.Dispose();
                Connetion.Close();

                Connetion = null;
            }


            var tmp = new List<T>(Devices.ToArray());
            foreach (var item in tmp)
            {
                item.CloseDevicesConnection();
                Devices.Remove(item);
            }
        }

        public virtual void AcceptCallbackDevices(IAsyncResult ar)
        {
            try
            {
                Socket handler = Connetion.EndAccept(ar);
                Devices.Add(new T());
                Devices.LastOrDefault().Start(handler);
                Connetion.BeginAccept(new AsyncCallback(AcceptCallbackDevices), null);


            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error, ex.Message);
            }


        }

        private void CreateDefaultSocket()
        {
            IPAddress ipAddress = IPAddress.Parse(Ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            Connetion = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Connetion.Bind(localEndPoint);
            Connetion.Listen(0);
            Connetion.BeginAccept(new AsyncCallback(AcceptCallbackDevices), null);


        }

        protected virtual string MessageDisconected(T item) => $"InjectWorker session closed about [{item.Connection.LocalEndPoint.ToString()}]";
        private void KillTimer_Tick(object sender, EventArgs e)
        {
            var tmp = new List<T>(Devices.ToArray());
            foreach (var item in tmp)
            {
                if (!item.isConected)
                {
                    Logs.Log.Write(Logs.TypeLog.Message, MessageDisconected(item));
                    item.CloseDevicesConnection();
                    Devices.Remove(item);
                }
            }
        }

        private void InitializationKillerTime()
        {
            if (KillTimer == null) KillTimer = new DispatcherTimer();
            KillTimer.Interval = MainSettings.CheckConnectServer;
            KillTimer.Tick += KillTimer_Tick;
            KillTimer.Start();
        }

        public void StartServer()
        {
            try
            {
                CreateDefaultSocket();
                InitializationKillerTime();
                Logs.Log.Write(Logs.TypeLog.Message, $"Inject stared at {Connetion.LocalEndPoint}!");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Logs.Log.Write(Logs.TypeLog.Error, e.Message);
            }
        }

    }
}
