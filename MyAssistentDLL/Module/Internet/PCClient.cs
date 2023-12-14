using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyAssistentDLL.Module.Internet
{
    internal class PCClient : InternetClient, IInternetClient
    {
        private class SendPak
        {
            public int id;
            public string Input_Cmd;
            public bool Out;public string Req;
        }
       
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
}
