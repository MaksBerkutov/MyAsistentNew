using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyAsistent
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var Splash = new SplashScreen("Loadin.jpg");
            Splash.Show(false);
            
            try
            {
                var item = new MainWindow();
                item.InilizationApp();
                item.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
            }
           
            
            Splash.Close(TimeSpan.FromSeconds(1));
        }
    }
}
