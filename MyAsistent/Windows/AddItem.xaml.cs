using System.Windows;
using MyAssistentDLL.Module.Codes;

namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddItem.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        public bool tryInit;
       public  (TypeArgumend, string[], bool local)? Result;
        public AddItem(string str)
        {

            InitializeComponent();
            if (tryInit = MainWindow.menu.TryGetValue(str, out MenuArgumentItem page))
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
            var item = (MenuArgumentItem)sender;
            if (item != null)
            {
                Result = item.Result;
            }
            this.Close();

        }
    }
}
