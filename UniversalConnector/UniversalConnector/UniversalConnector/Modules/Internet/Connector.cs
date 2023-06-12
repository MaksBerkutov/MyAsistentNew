using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace UniversalConnector.Modules.Internet
{
    public class Connection
    {
        Socket soc;

        public Connection(Socket soc)
        {
            this.soc = soc;
        }
    }
    public  class Connector
    {
       public static Connection GetConnection(string username,string passworld,string ip,int port)
        {
            

        }
    }
}
