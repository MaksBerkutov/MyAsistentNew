using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalConnector.Views;
using Xamarin.Forms;

namespace UniversalConnector.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string login = string.Empty;
        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }

        private string password = string.Empty;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
             
        }




        public Command LoginCommand { get; }
        public Command RegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked, CheckLoginClicked);
            RegisterCommand = new Command(OnRegisterClicked);

            this.PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }

        private  async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            if (App.Current.MainPage.GetType().Name == nameof(AppShell))
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            else 
                App.Current.MainPage = new AppShell();

        }
        private bool CheckLoginClicked(object obj) => login.Any() && password.Any();


        private async void OnRegisterClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            if (App.Current.MainPage.GetType().Name == nameof(AppShell))
                await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            else
                App.Current.MainPage = new AppShell();

        }
    }
}
