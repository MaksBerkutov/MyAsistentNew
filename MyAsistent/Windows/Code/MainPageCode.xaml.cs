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

namespace MyAsistent.Windows.Code
{
    /// <summary>
    /// Логика взаимодействия для MainPageCode.xaml
    /// </summary>
    public partial class MainPageCode : Page
    {
        private int CountLastFiles = 5;
        private FileCodeInfo selected;
        public MainPageCode()
        {
            var LastElement = Code.CodePage.Files.Skip(Math.Max(0, Code.CodePage.Files.Count - CountLastFiles)).ToList();
            InitializeComponent();
            foreach (var file in LastElement)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = file.Name;
                menuItem.Click += MenuItem_Click;
            }



        }
        private void Load(FileCodeInfo obj)
        {
            
            selected = obj;
            codeEditor.Text = System.IO.File.ReadAllText(selected.Path);
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Last 5 open
            var name = (sender as MenuItem).Header;
            var finded = Code.CodePage.Files.Find(x => x.Name.Equals(name));
            if (finded != null) Load(finded);

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            //Create
            var Dialog = new Code.CreateScriptDialog();
            Dialog.ShowDialog();
            if (Dialog.Result != null) Load(Dialog.Result);

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //save
            System.IO.File.WriteAllText(selected.Path, codeEditor.Text);
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            //debug
            ErrorBox.Text = String.Empty;
            if (MyAsistent.Module.CodeModul.Compiler.CodeCompiler.Compile(codeEditor.Text,out var err,out var memoryStream)){
                ErrorBox.Text = "Успешно скомпилиривано";
            }
            else
            {
                foreach (var ex in err) ErrorBox.Text += $"{ex.ToString()}\n";
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            //run
            MyAsistent.Module.CodeModul.Compiler.CodeCompiler.Run(codeEditor.Text);
        }
    }
}
