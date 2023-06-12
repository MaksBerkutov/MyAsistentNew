using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddVoiceCommandxaml.xaml
    /// </summary>
    public partial class AddVoiceCommandxaml : Page, Codes.MenuArgumentItem
    {
        private (Codes.TypeArgumend, string[], bool local)? result = null;
        public (Codes.TypeArgumend, string[], bool local)? Result => result;
        public static bool Try = false;
        public event Codes.Close OnClose;
        public AddVoiceCommandxaml()
        {
            InitializeComponent();
            Try = false;
        }
        public void clear()
        {
            result = null;
            Argumets.Text = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Argumets.Text.Length != 0)
            {
               
                result = (Codes.TypeArgumend.Voice, new string[] { Argumets.Text}, true);
            }
            Try = true;
            OnClose.Invoke(this);
            //Codes.Code_Saver.add((Command.Text, MainSettings.SpeechCulture), (Codes.TypeArgumend.Voice, new string[] { Argumets.Text }));
        }
    }
}
