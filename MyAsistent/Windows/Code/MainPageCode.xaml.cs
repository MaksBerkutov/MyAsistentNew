using MyAssistentDLL;
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
        private FileCodeInfo selected = null;
        public MainPageCode()
        {
            var LastElement = Code.CodePage.Files.Skip(Math.Max(0, Code.CodePage.Files.Count - CountLastFiles)).ToList();
            InitializeComponent();
            foreach (var file in LastElement)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = file.Name;
                menuItem.Click += MenuItem_Click;
                OldProject.Items.Add(menuItem);
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
            if (Dialog.Result != null) { 
                Load(Dialog.Result);
                if (OldProject.Items.Count > 1)
                    OldProject.Items.Remove(0);
                MenuItem menuItem = new MenuItem();
                menuItem.Header = Dialog.Result.Name;
                menuItem.Click += MenuItem_Click;
                OldProject.Items.Add(menuItem);
                
            }

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            //save
            if (selected != null)
                System.IO.File.WriteAllText(selected.Path, codeEditor.Text);
        }

        private async void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (selected != null)
                await Task.Run(() =>
                {
                    //Debug Button
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ErrorBox.Text = String.Empty;
                        if (ControllerAssistent.CompileCode(codeEditor.Text, out var err, out var memoryStream))
                            ErrorBox.Text = "Успешно скомпилиривано";
                        else
                            foreach (var ex in err)
                                ErrorBox.Text += $"{ex.ToString()}\n";
                        MenuItem_Click_2(sender, e);
                    });

                });

        }

        private async void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            //run
            if (selected != null)
                await Task.Run(() =>
                {

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        ErrorBox.Text = String.Empty;
                        if (ControllerAssistent.CompileCode(codeEditor.Text, out var err, out var memoryStream))
                        {
                            ErrorBox.Text = "Успешно скомпилиривано";
                            ControllerAssistent.RunCode(memoryStream);
                        }
                        else
                            foreach (var ex in err)
                                ErrorBox.Text += $"{ex.ToString()}\n";
                        MenuItem_Click_2(sender, e);
                    });
                });
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            //Exit
            MenuItem_Click_2(sender, e);
            codeEditor.Text = String.Empty;
            selected = null;
        }
    }
}
