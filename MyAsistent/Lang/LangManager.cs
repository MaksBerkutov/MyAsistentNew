using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyAsistent.Lang
{
    public class CultureLang
    {
        public string code { get; set; }    
        public string name { get; set; }
        public override string ToString() => name;
    }
    public class LanguageManager : INotifyPropertyChanged
    {
        public static readonly ObservableCollection<CultureLang> Lang = new ObservableCollection<CultureLang>()
        {
            new CultureLang(){name = "Українська", code ="ua"},
            new CultureLang(){name = "Англійська", code ="en"},
        };
        private string selectedLanguage;

        public string SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedLanguage)));
                    UpdateLanguageResources();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdateLanguageResources()
        {
            // Здесь вы можете выполнить логику для переключения языка и обновления ресурсов приложения

            // Пример: перезагрузка словаря ресурсов
            Application.Current.Resources.MergedDictionaries.Clear();
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri($"Lang/Resources.{SelectedLanguage}.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}
