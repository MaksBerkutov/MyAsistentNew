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

namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для ArduinoCommandCreate.xaml
    /// </summary>
    public partial class ArduinoCommandCreate : Page, Codes.MenuArgumentItem
    {
        private (Codes.TypeArgumend, string[], bool local)? result = null;
        public (Codes.TypeArgumend, string[], bool local)? Result => result;
        public static bool Try = false;

        public event Codes.Close OnClose;
        public ArduinoCommandCreate()
        {
            InitializeComponent();
            ArduinoName.ItemsSource = Module.Internet.InternetWorkerModule.DispetcherClassesConection.ArduinoClients;
            ArduinoName.SelectionChanged += ArduinoName_SelectionChanged;
        }

        private void ArduinoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((Module.Internet.ArduinoClient)ArduinoName.SelectedValue!=null)
                ArduinoCommand.ItemsSource = ((Module.Internet.ArduinoClient)ArduinoName.SelectedValue).CommandRec;
            else
            {
                ArduinoCommand.ItemsSource = null;
                ArduinoCommand.Items.Clear();
            }
        }
        public void clear()
        {
            result = null;
            ArduinoName.ItemsSource = Module.Internet.InternetWorkerModule.DispetcherClassesConection.ArduinoClients;
            Command.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            result = null;
            if (Command.Text.Length > 0  &&
                ArduinoCommand.SelectedIndex >= 0 && ArduinoName.SelectedIndex >= 0)
            {
                Try = true;
                result = (Codes.TypeArgumend.ArduinoReadDate, new string[] { ((Module.Internet.ArduinoClient)ArduinoName.SelectedValue).Name, ArduinoCommand.SelectedItem.ToString(),Command.Text }, true);
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
