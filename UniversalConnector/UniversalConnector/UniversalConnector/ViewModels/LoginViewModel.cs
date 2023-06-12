using System;
using System.Collections.Generic;
using System.Text;
using UniversalConnector.Views;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace UniversalConnector.ViewModels
{
    
    public class LoginViewModel : BaseViewModel
    {
        //public NewItemViewModel()
        //{
        //    SaveCommand = new Command(OnSave, ValidateSave);
        //    CancelCommand = new Command(OnCancel);
        //    this.PropertyChanged +=
        //        (_, __) => SaveCommand.ChangeCanExecute();
        //}

        //private bool ValidateSave()
        //{
        //    return !String.IsNullOrWhiteSpace(text)
        //        && !String.IsNullOrWhiteSpace(description);
        //}

        //public string Text
        //{
        //    get => text;
        //    set => SetProperty(ref text, value);
        //}

        //public string Description
        //{
        //    get => description;
        //    set => SetProperty(ref description, value);
        //}
        public Command LoginCommand { get; }
        private string _username;
        private string _password;
        private string _ip;
        private int _port;
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked, ValidCmd);
            this.PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }
        private bool ValidCmd(object obj)
        {
            return !String.IsNullOrWhiteSpace(_username)
                && !String.IsNullOrWhiteSpace(_username)
                && !String.IsNullOrWhiteSpace(_ip)
                && Port>0;
        }
        public string Username
        {
            get=> _username;
            set=> SetProperty(ref _username, value);
        }
        public string Passworld
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }
        public string Ip
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }
        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
          
                //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                App.Instance.MainPage = new AppShell();
           
           
        }
    }
}
