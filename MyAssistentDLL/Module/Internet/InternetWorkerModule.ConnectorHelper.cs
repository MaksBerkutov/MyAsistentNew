using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyAssistentDLL.Module.Internet
{

    public delegate void ConnectionEvent(string Name, IInternetClient Client);
    internal partial class InternetWorkerModule
    {
        internal class ConnectorHelper
        {
            
            private static event ConnectionEvent OnConnected;
            public static ConnectionEvent ConnectedEvent => OnConnected;
            bool IsRuning = true;
            Socket client;
            private string Getplatform()
            {
                client.Send(Encoding.ASCII.GetBytes(MainArduinoCommand.command[1]));
                while (IsRuning)
                    while (client.Available > 0)
                    {
                        var ByteArrayeForRead = new Byte[client.Available];
                        client.Receive(ByteArrayeForRead, ByteArrayeForRead.Length, SocketFlags.None);
                        return Encoding.ASCII.GetString(ByteArrayeForRead);

                    }
                return "Error";
            }
            public ConnectorHelper(Socket client)
            {
                this.client = client;
                Start();
            }

            private async Task AddArduinoClient()
            {
                var result =  await Task.Run( new Func<IInternetClient>(() =>
                {
                    DispetcherClassesConection.ArduinoClients.Add(new ArduinoClient(client));
                   return DispetcherClassesConection.ArduinoClients.Last();
                }));

                OnConnected?.Invoke("Arduino", result);
            }
            private async Task AddPCClient()
            {
                var result = await Task.Run(new Func<IInternetClient>(() =>
                {
                    DispetcherClassesConection.PCClients.Add(new PCClient(client));
                    return DispetcherClassesConection.PCClients.Last();
                }));

                OnConnected?.Invoke("PC", result);
            }
            private async void Start()
            {
                await Task.Run(async () =>
                {
                    

                    Timer dm = new Timer(_ => { IsRuning = false; }, null, 0, (int)MainSettings.WaitForConectionDevice.TotalMilliseconds);

                    string Platform = Getplatform();
                    mutex.WaitOne();


                    if (Platform == "Arduino")
                        await AddArduinoClient();
                    else if (Platform == "PC")
                        await AddPCClient();
                    else if (Platform == "Error")
                        Logs.Log.Write(Logs.TypeLog.Error, "Плата не была иницилизирована!");

                    mutex.ReleaseMutex();
                });
            }

       
        }

    }
}
