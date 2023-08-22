using MyAsistent.Module.Internet.CodeInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Security.Cryptography;

namespace MyAsistent.Module.Internet.UniversalConnetion
{
    class UniversalConnectorDevice:Model.Devices
    {

        
        protected override void HandlerCommand(Model.ObjectDevice DeserelizeData) { 
        
        }
       
        
    }
    class UniversalConnectorServer: Model.Server<UniversalConnectorDevice>
    {
        public UniversalConnectorServer()
        {
            this.Ip = MainSettings.IpInject;
            this.Port = 456;
        }
        protected override string MessageStart() => $"Universal Connector Server stared at {Connetions.LocalEndPoint}!";



    }
}
