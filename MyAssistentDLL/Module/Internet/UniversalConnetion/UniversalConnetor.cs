
using MyAsistentDLL.Module.Internet.Model;

namespace MyAssistentDLL.Module.Internet.UniversalConnetion
{
    internal class UniversalConnectorDevice : Devices
    {
        public void SendMessage(string message)
        {
            this.SendPacket(Newtonsoft.Json.JsonConvert.SerializeObject(new string[] { "msgbox", message }));
        }

        protected override void HandlerCommand(ObjectDevice DeserelizeData)
        {

        }


    }
    internal class UniversalConnectorServer : Server<UniversalConnectorDevice>
    {
        public UniversalConnectorServer()
        {
            this.Ip = MainSettings.IpInject;
            this.Port = 456;
        }
        protected override string MessageStart() => $"Universal Connector Server stared at {Connetions.LocalEndPoint}!";



    }
}
