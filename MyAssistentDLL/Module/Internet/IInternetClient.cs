using System.Net.Sockets;

namespace MyAssistentDLL.Module.Internet
{
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


    internal abstract class InternetClient
    {

        public InternetClient(Socket socket)
        {

        }



    }
}
