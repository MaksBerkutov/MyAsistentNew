using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace UniversalConnector.ViewModels
{
    class ConnectorViewModel:BaseViewModel
    {
        private string ip = "192.168.1.200";
        private int port = 456;
        private Module.InternetConnect internetConnect;
        public ICommand OpenSocketCommand { get; }
        public ConnectorViewModel()
        {
            Title = "Connect";
            OpenSocketCommand = new Command(() => internetConnect= new Module.InternetConnect(ip,port));

        }
        public string Ip
        {
            get => ip;
            set => SetProperty(ref ip, value);
        }

        public int Port
        {
            get => port;
            set => SetProperty(ref port, value);
        }
    }
}
