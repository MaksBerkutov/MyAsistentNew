using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyAssistentDLL;
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL.Module.Internet;


namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для ArduinoCommandCreate.xaml
    /// </summary>
    public partial class ArduinoNonResult : Page, MenuArgumentItem
    {
        private (TypeArgumend, string[], bool local)? result = null;
        public (TypeArgumend, string[], bool local)? Result => result;
        private void Load()
        {
            var AllArduino = ControllerAssistent.GetAllArduinoClinet();

            AllArduino.ForEach(ar =>
            ArduinoName.Items.Add(ar)
            );

        }
        public ArduinoNonResult()
        {
            InitializeComponent();
            ControllerAssistent.OnConnetctedDevicesInternet += ControllerAssistent_OnConnetctedDevicesInternet;
            Load();
            ArduinoName.SelectionChanged += ArduinoName_SelectionChanged;
        }

        private void ControllerAssistent_OnConnetctedDevicesInternet(string Name, MyAssistentDLL.Module.Internet.IInternetClient Client)
        {
            if (Name == "Arduino")
            {
                ArduinoName.Items.Add(Client);

            }
        }

        public static bool Try = false;
        public void clear()
        {
            result = null;
            Load();
        }
        public event Close OnClose;

        private void ArduinoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ArduinoName.SelectedValue as IInternetClient;

            if (selected != null)
                ArduinoCommand.ItemsSource = ControllerAssistent.GetCommandArduino(selected);
            else
            {
                ArduinoCommand.ItemsSource = null;
                ArduinoCommand.Items.Clear();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (
                ArduinoCommand.SelectedIndex >= 0 && ArduinoName.SelectedIndex >= 0 )
            {
                Try = true;
                result = (TypeArgumend.Arduino, new string[] { (ArduinoName.SelectedValue as IInternetClient).Name, ArduinoCommand.SelectedItem.ToString() }, true);
                OnClose.Invoke(this);
            }
        }

        private void Voice_TextChanged()
        {

        }
    }
}
