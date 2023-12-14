
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL;

namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateCommandBox.xaml
    /// </summary>
    public partial class CreateCommandBox : Window
    {
        public ObservableCollection<(TypeArgumend, string[], bool local)> Result = new ObservableCollection<(TypeArgumend, string[], bool local)>();

        public CreateCommandBox()
        {
            InitializeComponent();
            AllCMD.ItemsSource = Result;
        }

        private void HandlerClick(object sender, RoutedEventArgs e)
        {
            AddItem addItem = new AddItem(((Button)sender).Name);
            addItem.ShowDialog();
                if(addItem.Result!=null)
                    Result.Add(addItem.Result.Value);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CodeSaver.add((CommandTEXT.Text, MainSettings.SpeechCulture), Result.ToList());
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (AllCMD.SelectedIndex >= 0)
                Result.Remove((((TypeArgumend, string[], bool local))AllCMD.SelectedItem));
        }
    }
}
