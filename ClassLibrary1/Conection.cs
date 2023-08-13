using System;
using System.Net.Sockets;
using System.Text;
 
namespace Conectector
{
    public class Conection
    {
        public delegate void NewPakaje(Worker.Worker.Pakage pak);
        public delegate void NewMess(string pak);

        public event NewPakaje OnNewPakaje;
        public event NewMess OnNewMsg;

        private Socket Conect;
        private string AuthKey;
       
        public Conection(Socket Conect, string authKey)
        {
            this.Conect = Conect;
            AuthKey = authKey;
        }

        public Worker.Worker GetWorker() => new Worker.Worker(this);

        public int Receive(byte[]arr)=>Conect.Receive(arr);
        public int Available => Conect.Available;
       
        public void SignaturePakage(ref Worker.Worker.Pakage obj) => obj.Key = AuthKey;
        public void SendPackaje(string json) => Conect.Send(Encoding.UTF8.GetBytes (json));
    }
}


