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

namespace MyAsistent.Module.MessageBoxes
{
    /// <summary>
    /// Логика взаимодействия для GetString.xaml
    /// </summary>
    public partial class GetString : Window
    {
        public string result = string.Empty;
        public GetString(string ti,string te,string st = " ")
        {
            InitializeComponent();
            this.Title = ti;
            this.Blocks.Text = te;
            this.tb.Text = st;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            result = tb.Text;   
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
