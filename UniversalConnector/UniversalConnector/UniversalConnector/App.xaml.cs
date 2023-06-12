using System;
using UniversalConnector.Services;
using UniversalConnector.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UniversalConnector
{
    public partial class App : Application
    {
        public static App Instance { get;  private set; }
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            Instance = this;
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
