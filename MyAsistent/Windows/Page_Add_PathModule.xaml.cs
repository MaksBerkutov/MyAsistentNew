using Codes;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                result = (Codes.TypeArgumend.Path, new string[] { PathI.Text }, LocalMachine.IsChecked.Value);

            else Logs.Log.Write(Logs.TypeLog.Warning, "Path is NULL");
            Try = true;
            Content = null;
            OnClose.Invoke(this);
            
        }

        private void PathI_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
