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
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL.Module.Internet;
using MyAssistentDLL;


namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для ArduinoCommandCreate.xaml
    /// </summary>
    public partial class ArduinoCommandCreate : Page, MenuArgumentItem
    {
        private (TypeArgumend, string[], bool local)? result = null;
        public (TypeArgumend, string[], bool local)? Result => result;
        public static bool Try = false;

        public event Close OnClose;
        private void Load()
        {
            var AllArduino = ControllerAssistent.GetAllArduinoClinet();

            AllArduino.ForEach(ar =>
            ArduinoName.Items.Add(ar)
            );

        }
        public ArduinoCommandCreate()
        {
            InitializeComponent();
            Load();
            ControllerAssistent.OnConnetctedDevicesInternet += ControllerAssistent_OnConnetctedDevicesInternet;
            ArduinoName.SelectionChanged += ArduinoName_SelectionChanged;
        }

        private void ControllerAssistent_OnConnetctedDevicesInternet(string Name, IInternetClient Client)
        {
            if (Name == "Arduino")
            {
                ArduinoName.Items.Add(Client);

            }
        }

        private void ArduinoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ArduinoName.SelectedValue as IInternetClient;
            if (selected != null)
                ArduinoCommand.ItemsSource = ControllerAssistent.GetRecCommandArduino(selected);
            else
            {
                ArduinoCommand.ItemsSource = null;
                ArduinoCommand.Items.Clear();
            }
        }
        public void clear()
        {
            result = null;
            Load();
            Command.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            result = null;
            if (Command.Text.Length > 0  &&
                ArduinoCommand.SelectedIndex >= 0 && ArduinoName.SelectedIndex >= 0)
            {
                Try = true;
                result = (TypeArgumend.ArduinoReadDate, new string[] { (ArduinoName.SelectedValue as IInternetClient).Name, ArduinoCommand.SelectedItem.ToString(),Command.Text }, true);
                OnClose.Invoke(this);

            }
        }

        private void Voice_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

       
    }
}
