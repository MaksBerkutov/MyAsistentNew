using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyAsistent.Windows.Code
{
    /// <summary>
    /// Логика взаимодействия для CreateScriptDialog.xaml
    /// </summary>
    public partial class CreateScriptDialog : Window
    {
        public FileCodeInfo Result = null;
        public CreateScriptDialog()
        {
            InitializeComponent();
            Path.Text = MainSettings.PathToSaveScript;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Open Path
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    Path.Text = folderBrowserDialog.SelectedPath;
                  
                }
                else
                {
                    
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Save
            if (!File.Text.Any() || !Path.Text.Any()) return;
            if (!File.Text.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) File.Text += ".cs";
            Result = new FileCodeInfo() { Name = File.Text, Path = $@"{Path.Text}\{File.Text}" };
            try
            {
                System.IO.File.WriteAllText(Result.Path, @"
            using System;
            class Program{
                public static void Main(){
                   //Default Code
                }
            }
            
        ");
                CodePage.Files.Add(Result);
              
            }
            catch (Exception ex)
            {
                Logs.Log.Write(Logs.TypeLog.Error,ex.Message);
                
            }
            
        }
    }
}
