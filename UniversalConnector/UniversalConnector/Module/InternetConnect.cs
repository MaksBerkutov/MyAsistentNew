using RSACryptoServiceProviderExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UniversalConnector.Interface;
using Xamarin.Forms;

namespace RSACryptoServiceProviderExtensions
{
    public static class RSACryptoServiceProviderExtensions
    {
        public static void FromXmlStringTest(this RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlStringTest(this RSACryptoServiceProvider rsa, RSAParameters parameters)
        {
            if(parameters.DP == null)
            {
                return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
              Convert.ToBase64String(parameters.Modulus),
              Convert.ToBase64String(parameters.Exponent));
              
            }
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent),
                Convert.ToBase64String(parameters.P),
                Convert.ToBase64String(parameters.Q),
                Convert.ToBase64String(parameters.DP),
                Convert.ToBase64String(parameters.DQ),
                Convert.ToBase64String(parameters.InverseQ),
                Convert.ToBase64String(parameters.D));
        }
    }
}


namespace UniversalConnector.Module
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

    public class RSAextentions
    {
        public static string GenerateKeys(ref string PrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {

                PrivateKey = rsa.ToXmlStringTest(rsa.ExportParameters(true));
                return rsa.ToXmlStringTest(rsa.ExportParameters(false));
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
                rsa.FromXmlStringTest(PrivateKey);
                byte[] encryptedData = Convert.FromBase64String(encryptedText);
                byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
    public class InternetConnect
    {
        public Socket Connection { get; private set; }
        private string PublicKey;
        private string PrivateKey;
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
        public delegate void InputCommand(object Date);
        private bool CheckConnected()
        {
            bool part1 = Connection.Poll(1000, SelectMode.SelectRead);
            bool part2 = (Connection.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
        public InternetConnect(string Ip,int Port)
        {
            IPAddress ipAddr = IPAddress.Parse(Ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, Port);

            Connection = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Connection.Connect(localEndPoint);
            this.Connection.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, this.Connection);
        }

        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        private void SendPacket(string Date)
        {
            Date = Date == string.Empty ? string.Empty : RSAextentions.EncryptWithPublicKey(Date, PublicKey);
            Connection.Send(Encoding.UTF8.GetBytes(
             new PacketDevice() { Date = Date, PublicKey = RSAextentions.GenerateKeys(ref PrivateKey) }
             .ToString()));
        }

        public void CloseDevicesConnection()
        {
            Connection.Dispose();
        }

        private void ConvertDatePackets(PacketDevice InputPacket)
        {

            if (InputPacket != null && InputPacket.PublicKey.Any())
            {
                this.PublicKey = InputPacket.PublicKey;
                if(InputPacket.Date.Length == 0) this.SendPacket(string.Empty);
            }
            else return;
            if (InputPacket.Date.Any())
            {
                var decryptedData = RSAextentions.DecryptWithPrivateKey(InputPacket.Date, this.PrivateKey);
                ObjectDevice deserelizeData = new ObjectDevice(Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(decryptedData));
                HandlerCommand(deserelizeData);
            }

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
                Connection.Close();
            }

        }
        private void HandlerCommand(ObjectDevice Obj)
        {
            switch (Obj.Command) {
                case "msgbox": 
                    {
                        DependencyService.Get<IPushNotificationService>().HandleNotification($"{Obj.Args?[0]}");
                        break;
                    }
                case "aunt":
                    {
                        LoginCommandOutput(bool.Parse(Obj.Args?[0]));
                        break;
                    }
            
            }

        }


        //Send Function
        public static event InputCommand LoginCommandOutput;
        public void SendLoginCommand(string Login, string Password)
        {

        }

    }
}
