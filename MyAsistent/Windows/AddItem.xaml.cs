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
    /// Логика взаимодействия для AddItem.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        public bool tryInit;
       public  (Codes.TypeArgumend, string[], bool local)? Result;
        public AddItem(string str)
        {

            InitializeComponent();
            if (tryInit = MainWindow.menu.TryGetValue(str, out Codes.MenuArgumentItem page))
            {
                page.OnClose += Page_OnClose;
                page.clear();
                Content = page.Content;
                Result = page.Result;
            }
            else this.Close();

        }

        private void Page_OnClose(object sender)
        {
            var item = ((Codes.MenuArgumentItem)sender);
            if (item != null)
            {
                Result = item.Result;
            }
            this.Close();

        }
    }
}
