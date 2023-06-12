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
    public partial class ArduinoNonResult : Page, Codes.MenuArgumentItem
    {
        private (Codes.TypeArgumend, string[], bool local)? result = null;
        public (Codes.TypeArgumend, string[], bool local)? Result => result;
        public ArduinoNonResult()
        {
            InitializeComponent();
            ArduinoName.ItemsSource = Module.Internet.InternetWorkerModule.DispetcherClassesConection.ArduinoClients;
            ArduinoName.SelectionChanged += ArduinoName_SelectionChanged;
        }
        public static bool Try = false;
        public void clear()
        {
            result = null;
            ArduinoName.ItemsSource = Module.Internet.InternetWorkerModule.DispetcherClassesConection.ArduinoClients;
        }
        public event Codes.Close OnClose;

        private void ArduinoName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((Module.Internet.ArduinoClient)ArduinoName.SelectedValue!=null)
                ArduinoCommand.ItemsSource = ((Module.Internet.ArduinoClient)ArduinoName.SelectedValue).Command;
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
                result = (Codes.TypeArgumend.Arduino, new string[] { ((Module.Internet.ArduinoClient)ArduinoName.SelectedValue).Name, ArduinoCommand.SelectedItem.ToString() }, true);
                OnClose.Invoke(this);
            }
            //Codes.Code_Saver.add((Command.Text, MainSettings.SpeechCulture), (Codes.TypeArgumend.Arduino, new string[] { ((Module.Internet.ArduinoClient)ArduinoName.SelectedValue).Name, ArduinoCommand.SelectedItem.ToString(), Voice.Text }));
        }

        private void Voice_TextChanged()
        {

        }
    }
}
