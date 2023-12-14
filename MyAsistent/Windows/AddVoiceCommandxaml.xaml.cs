using System.Windows;
using System.Windows.Controls;
using MyAssistentDLL.Module.Codes;


namespace MyAsistent.Windows
{
   
    public partial class AddVoiceCommandxaml : Page, MenuArgumentItem
    {
        private (TypeArgumend, string[], bool local)? result = null;
        public (TypeArgumend, string[], bool local)? Result => result;
        public static bool Try = false;
        public event Close OnClose;
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
               
                result = (TypeArgumend.Voice, new string[] { Argumets.Text}, true);
            }
            Try = true;
            OnClose.Invoke(this);
        }
    }
}
