using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using MyAssistentDLL;
using MyAssistentDLL.Logs;


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
            Path.Text = MainSettings.PATH_SAVE_SCRIPT;
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
                this.Close();
              
            }
            catch (Exception ex)
            {
                Log.Write(TypeLog.Error,ex.Message);
                
            }
            
        }
    }
}
