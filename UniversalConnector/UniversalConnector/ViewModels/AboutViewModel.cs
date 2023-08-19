using System;
using System.Windows.Input;
using UniversalConnector.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UniversalConnector.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            CreateNotify = new Command( () => DependencyService.Get<IPushNotificationService>().HandleNotification($"Tested Message date time {DateTime.Now.ToLocalTime()}"));
        }

        public ICommand OpenWebCommand { get; }
        public ICommand CreateNotify { get; }
    }
}