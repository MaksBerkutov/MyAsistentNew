using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Interpretator
{
    public static class Interpretator
    {
        class KeyboardEmu
        {
            public List<Key> keys;
            public KeyboardEmu() => keys = new List<Key>();
            public KeyboardEmu(List<Key> Keys) => keys = Keys;
            public override string ToString() => String.Join("@", keys.ToArray());

            public static bool tryParse(string str, out KeyboardEmu obj)
            {
                obj = new KeyboardEmu();
                try
                {

                    var items = str.Split('@');
                    foreach (var item in items)
                        obj.keys.Add((Key)(Enum.Parse(typeof(Key), item)));
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }
            }

        }
        private static void emulatedKeyboard(KeyboardEmu obj)
        {
            if (obj.keys == null || obj.keys.Count == 0) return;
            if (obj.keys.Count > 1)
            {
                string command = "";
                foreach (var key in obj.keys)
                {
                    switch (key)
                    {
                        case System.Windows.Input.Key.LeftShift:
                        case System.Windows.Input.Key.RightShift:
                            command += "+";
                            break;
                        case System.Windows.Input.Key.LeftCtrl:
                        case System.Windows.Input.Key.RightCtrl:
                            command += "^";
                            break;
                        case System.Windows.Input.Key.LeftAlt:
                        case System.Windows.Input.Key.RightAlt:
                            command += "%";
                            break;
                        case System.Windows.Input.Key.F1:
                        case System.Windows.Input.Key.F2:
                        case System.Windows.Input.Key.F3:
                        case System.Windows.Input.Key.F4:
                        case System.Windows.Input.Key.F5:
                        case System.Windows.Input.Key.F6:
                        case System.Windows.Input.Key.F7:
                        case System.Windows.Input.Key.F8:
                        case System.Windows.Input.Key.F9:
                        case System.Windows.Input.Key.F10:
                        case System.Windows.Input.Key.F11:
                        case System.Windows.Input.Key.F12:
                            command += "{" + key.ToString() + "}";
                            break;
                        default: command += key.ToString(); break;
                    }
                }
                SendKeys.SendWait(command);
            }
            else SendKeys.SendWait(obj.keys[0].ToString());
        }
        public static void Go((string,string[]) Arguments)
        {
            switch (Arguments.Item1)
            {

                case "Path":
                    foreach (var i in Arguments.Item2)
                        Process.Start(new ProcessStartInfo(i));
                    break;
     
   

                case "Keyboard":
                    foreach (var i in Arguments.Item2)
                    {
                        if (KeyboardEmu.tryParse(i, out KeyboardEmu variable))
                            emulatedKeyboard(variable);
                        else continue;
                    }
                    break;
                case "Command":
                    break;
                default:
                    break;
            }
        }
    }
}
namespace Connection
{
    public interface IConnection
    {
        void Connect(string[]args);
        void Disconnect();
        void Start();
    }
    public class COMConnection : IConnection
    {
        public void Connect(string[] args)
        {
            //args[0] == Name Com to Connect;
        }

        public void Disconnect()
        {
           //disconect
        }

        public void Start()
        {
           //connect
        }
    }
    public class WIFIConnection : IConnection
    {
        class SendToPC
        {
            public string Name;
            public string[] Args;

            public SendToPC(string name, string[] args)
            {
                Name = name;
                Args = args;
            }
        }
        private static Socket sender;
        private const int BUFFER_SIZE = 2048;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];

        public void Connect(string[] args)
        {
            try
            {

                IPHostEntry ipHost = Dns.GetHostEntry(args[0]);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, int.Parse(args[1]));

                sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);
               

                try
                {
                     sender.Connect(localEndPoint);
                     sender.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, sender);


                }

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
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SendToPC>(strJson);
            Interpretator.Interpretator.Go((result.Name, result.Args));

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }
        public void Disconnect()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
namespace ConnectorPC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0) return;
            Connection.IConnection connection = null;
            switch (args[0])
            {
                case "WIFI":
                    if (args.Length != 3) return;
                    connection = new Connection.WIFIConnection();
                    connection.Connect(new string[]{args[1],args[2] });
                    break;
                case "COM":
                    connection = new Connection.COMConnection();
                    break;
            }
            if (connection == null) return;
            connection.Start();
            Console.ReadLine();
            connection.Disconnect();
        }
    }
}
