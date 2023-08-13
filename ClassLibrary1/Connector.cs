using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace InjectionAsistent
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
    public class Injection
    {

        private string Ip;
        private int Port;
        private string PrivateKey;
        private string PublicKey;
        private string Login;
        private string Password;
        public string SessionLogin { get; private set; }

        private Socket Connection;

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

        private string GenerateKeys()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {

                PrivateKey = rsa.ToXmlString(true);
                return rsa.ToXmlString(false);
            }
        }

        private void SendPacket(string Date)
        {
            Date = Date == string.Empty ? string.Empty : EncryptWithPublicKey(Date);
            Connection.Send(Encoding.UTF8.GetBytes(
             new PacketDevice() { Date = Date, PublicKey = GenerateKeys() }
             .ToString()));
        }
        
        private PacketDevice DecryptDate(PacketDevice Encrypt)
        {
            if(Encrypt != null && Encrypt.Date.Length != 0)
            {
                Encrypt.Date = this.DecryptWithPrivateKey(Encrypt.Date); 
            }
            return Encrypt;
        }

        private PacketDevice ReadDate()
        {
            while (true)//Wait Answer Server
                while (Connection.Available > 0)
                {
                    byte[] messageReceived = new byte[Connection.Available];
                    Connection.Receive(messageReceived);
                    var readString = Encoding.UTF8.GetString(messageReceived);
                    return PacketDevice.ConvertFromString(readString);

                }
        }
        public Injection(string Login, string Password, string Ip = "127.0.0.1", int Port = 2745)
        {
            this.Login = Login;
            this.Password = Password;

            this.Ip = Ip;
            this.Port = Port;


        }
        private void ConnectToServer()
        {
            IPAddress ipAddr = IPAddress.Parse(Ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, Port);

            Connection = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Connection.Connect(localEndPoint);
            PublicKey = ReadDate().PublicKey ;
        }
        private void Auntification()
        {
            SendPacket(Newtonsoft.Json.JsonConvert.SerializeObject(new string[] { "Authentication", Login, Password }));
            var decrypt = this.DecryptDate(ReadDate());
            if (decrypt.Date == "false") throw new Exception("Error not login check Login and Pasword");
            else this.SessionLogin = decrypt.Date;

        }

        public void Start()
        {
            ConnectToServer();
            Auntification();
        }
    }
    
}











