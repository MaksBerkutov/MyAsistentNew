using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL.Logs;

namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для Page_Add_PathModule.xaml
    /// </summary>
    public partial class Page_Add_PathModule : Page, MenuArgumentItem
    {
        public Page_Add_PathModule()
        {
            InitializeComponent();
            
            Try = false;
           
        }
        public void clear()
        {
            result = null;
            LocalMachine.IsChecked = true;
            PathI.Text = "";
        }
        private (TypeArgumend, string[], bool local)? result = null;
        public (TypeArgumend, string[], bool local)? Result => result;
        private void PathButI_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "EXE Files (*.exe)|*.exe|bat files (*.bat*)|*bat*";
            if (ofd.ShowDialog() == true)
            {
                PathI.Text = ofd.FileName;
            }
        }
        public static bool Try = false;

        public event Close OnClose;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (!(PathI.Text == ""))
                result = (TypeArgumend.Path, new string[] { PathI.Text }, LocalMachine.IsChecked.Value);

            else Log.Write(TypeLog.Warning, "Path is NULL");
            Try = true;
            Content = null;
            OnClose.Invoke(this);
            
        }

        private void PathI_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
