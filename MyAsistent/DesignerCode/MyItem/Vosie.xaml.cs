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

namespace MyAsistent.Module.DesignerCode.MyItem
{
    /// <summary>
    /// Логика взаимодействия для Vosie.xaml
    /// </summary>
    public partial class Vosie : UserControl,ICode
    {
        //Voise
        public string Voise { get; set; } = string.Empty;
        public Vosie()
        {
            InitializeComponent();
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myPopup.IsOpen = true;
        }

        public string GetCode()
        {
            return $"    MyAsistent.Module.CodeModul.Compiler.AsisstentCodeBase.Voise(\"{Voise}\");";
        }

        public UserControl GetUserControl()
        {
            return this;
        }
        public ICode GetNewElement() => new Vosie();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Voise = txtVoise.Text;

            
            myPopup.IsOpen = false;
        }

        public event Delete OnDelete;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this);
        }

        public void Load(object[] arg)
        {
            Voise = txtVoise.Text = arg[1].ToString();
        }

        public object[] Save()
        {
            return new object[] { typeof(Vosie), Voise };
        }
    }
}
