using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace MyAsistent.Module.Internet.CodeInject
{

    public class InjectionDevice: Model.Devices
    {
        public string SessionLogin { get; set; }

        public override void CloseDevicesConnection()
        {
            Account.CloseConnection(SessionLogin);
            Connection.Dispose();
        }

        public InjectionDevice()
        {
           
        }

        private void Authentication(Account Arg)
        {
            if (Arg == null || Arg.Connection)
            {
                this.SendPacket("false");
                this.Connection.Dispose();
                Connection = null;
                return;
            }
            SessionLogin = Arg.Login;
            this.SendPacket(Arg.Login);
        }
        private void Authentication(List<string> Arg)
        {
            if(Arg.Count == 2)
                Authentication(Account.FindUser(Arg[0], Arg[1]));
                /*
                0 index = Login
                1 index = Password
                */
        }

        protected override void HandlerCommand(Model.ObjectDevice Obj)
        {
            switch (Obj.Command)
            {
                case "Authentication":
                    Authentication(Obj.Args);
                    break;
                case "Voice":
                    Sound.Voice.PlayRuAsync(Obj.Args[0]);
                    break;
                case "Arduino":
                    InternetWorkerModule.SendToClent(Obj.Args[0], Obj.Args[1]);
                    break;
                case "ArduinoDate":
                    if (InternetWorkerModule.SendToClinetRecive(Obj.Args[0], Obj.Args[1], out var res))
                        this.SendPacket(res);
                    else
                        this.SendPacket("NULL");
                    break;


            }
        }

     
    }

    public class InjectionServer: Model.Server<InjectionDevice>
    {
        public InjectionServer()
        {
            this.Ip = MainSettings.IpInject;
            this.Port = MainSettings.PortInject;
        }
        protected override string MessageStart() => $"Inject stared at {Connetions.LocalEndPoint}!";
        protected override string MessageDisconected(InjectionDevice item) => $"InjectWorker session closed about [{item.SessionLogin}]";

    }
}
