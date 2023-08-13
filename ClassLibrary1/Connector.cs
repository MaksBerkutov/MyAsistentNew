using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Connector
{
    struct SocketManagment
    {
        public Socket sender;
        public IPEndPoint localEndPoint;

        public SocketManagment(Socket sender, IPEndPoint localEndPoint)
        {
            this.sender = sender;
            this.localEndPoint = localEndPoint;
        }


    }

    public class Connector
    {

        private string Ip;
        private int Port;
        private string Login;
        private string Password;
      
        public Connector(string Login, string Password, string Ip = "127.0.0.1", int Port = 2745)
        {
            this.Login = Login;
            this.Password = Password;
           
                this.Ip = Ip;
                this.Port = Port;
            

        }
        private SocketManagment CreateDefautSocket()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Ip);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, Port);

            return new SocketManagment( new Socket(ipAddr.AddressFamily,SocketType.Stream, ProtocolType.Tcp),
                        localEndPoint);
        }
        private int SendAuntificationMessage(SocketManagment obj)
        {
            obj.sender.Connect(obj.localEndPoint);
            byte[] messageSent = Encoding.ASCII.GetBytes(
                Newtonsoft.Json.JsonConvert.SerializeObject(
                    new Worker.Worker.Pakage { Reqest = $"Auth={Login},{Password}" }));
            return obj.sender.Send(messageSent);
        }
        private Conectector.Conection CheckResultServer(byte[] messageReceived,Socket sender)
        {
            var Result = Newtonsoft.Json.JsonConvert.DeserializeObject<Worker.Worker.Pakage>(Encoding.ASCII.GetString(messageReceived));
            if (Result.Resume == "false") return null;
            else return new Conectector.Conection(sender, Result.Resume);
        }
        private Conectector.Conection CreateConnection()
        {
            var resultSocket = CreateDefautSocket();


            int byteSent = SendAuntificationMessage(resultSocket);
            while (true)//Wait Answer Server
                while (resultSocket.sender.Available > 0)
                {
                    byte[] messageReceived = new byte[resultSocket.sender.Available];
                    resultSocket.sender.Receive(messageReceived);
                    return CheckResultServer(messageReceived, resultSocket.sender);

                }
        }
        public Conectector.Conection GetConnection()
        {
            try
            {
                return CreateConnection();
               
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
            return null;

        }
        public async Task<Conectector.Conection> GetConectionAsync()
        {
            return await Task.Run(() => GetConnection());
        }
    }
}










