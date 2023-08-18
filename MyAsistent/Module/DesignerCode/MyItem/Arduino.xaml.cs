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
    /// Логика взаимодействия для Arduino.xaml
    /// </summary>
    public partial class Arduino : UserControl,ICode
    {
        public string NamePlate { get; set; } = string.Empty; 
        public string Command { get; set; } = string.Empty;
        public Arduino()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myPopup.IsOpen = true;
        }

        public string GetCode()
        {
            return $"    MyAsistent.Module.CodeModul.Compiler.AsisstentCodeBase.SendToArduino(\"{NamePlate}\",\"{Command}\");";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
             NamePlate = txtName.Text;
             Command = txtSurname.Text;

             myPopup.IsOpen = false;
        }

        public UserControl GetUserControl()
        {
            return this;
        }

        public ICode GetNewElement() => new Arduino();

        public event Delete OnDelete;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnDelete?.Invoke(this);
        }

        public void Load(object[] arg)
        {
            NamePlate = txtName.Text = arg[1].ToString();
            Command = txtSurname.Text = arg[2].ToString();
        }

        public object[] Save()
        {
            return new object[] {typeof(Arduino), NamePlate, Command };
        }
    }
}
