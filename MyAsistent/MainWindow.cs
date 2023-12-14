using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using MyAssistentDLL;
using MyAssistentDLL.Logs;
using MyAssistentDLL.Module.Internet;


namespace MyAsistent
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public static class Extentions
    {
        public static void removeDuplicates<T>(this List<T> list)
        {
            HashSet<T> hashset = new HashSet<T>();
            list.RemoveAll(x => !hashset.Add(x));
        }
    }
    public class MainWindow_ViewModel : INotifyPropertyChanged
    {
        private int bhaud;
        private int port;
        private int alis;//AddresListIDServer
        private string ip;
        private string ssid;
        private string passworld;
        private string mainPref;
        private string mainFold;
        private bool statusWebServers;
        private ObservableCollection<IPAddress> allIpAdrres = new ObservableCollection<IPAddress>();
        public ObservableCollection<IPAddress> AllIpAdress =>allIpAdrres;

        //telegram
        private string apikey;
        private bool statusTBot;

        //inject
        private bool acceptInject;
        private string ipInject;
        private int portInject;


        private void loadIps()
        {
            try
            {
                var tmp  = new List<IPAddress>(Dns.GetHostEntry(MainSettings.IPServer).AddressList);
                tmp.ForEach(x => allIpAdrres.Add(x));
            }
            catch (Exception ex)
            {
                allIpAdrres.Clear();
                Log.Write(TypeLog.Error,ex.Message);
            }
            
        }
        public MainWindow_ViewModel() { 
            loadIps();
            Bhaud = MainSettings.BaudRate; OnPropertyChanged("Bhaud");
            Alis = MainSettings.AddresListIDServer; OnPropertyChanged("Alis");
            Port = MainSettings.PortServer; OnPropertyChanged("Port");
            Ip = MainSettings.IPServer; OnPropertyChanged("Ip");
            Ssid = MainSettings.SSID; OnPropertyChanged("Ssid");
            AcceptInject = MainSettings.StatusInject; OnPropertyChanged("AcceptInject");
            MainPref = MainSettings.MainPrefix; OnPropertyChanged("MainPref");
            StatusWebServers = MainSettings.StatusWebServer; OnPropertyChanged("StatusWebServers");
            MainFold = MainSettings.MainFolder; OnPropertyChanged("MainFold");
            Apikey = MainSettings.APIKey; OnPropertyChanged("Apikey");
            StatusTBot = MainSettings.StatusTBot; OnPropertyChanged("StatusTBot");
            IpInject = MainSettings.IpInject; OnPropertyChanged("IpInject");
            PortInject = MainSettings.PortInject; OnPropertyChanged("PortInject");
        }
        public void SyncronaizeWebGui()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MainPref = MainSettings.MainPrefix; OnPropertyChanged("MainPref");
                StatusWebServers = MainSettings.StatusWebServer; OnPropertyChanged("StatusWebServers");
                MainFold = MainSettings.MainFolder; OnPropertyChanged("MainFold");
            });
            
        }
        public void SyncronaizeTelegramGui()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Apikey = MainSettings.APIKey; OnPropertyChanged("Apikey");
                StatusTBot = MainSettings.StatusTBot; OnPropertyChanged("StatusTBot");
            });

        }
        public void SyncronaizeInjection()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                AcceptInject = MainSettings.StatusInject; OnPropertyChanged("AcceptInject");
            });

        }
        public void SyncronaizeServerGui()
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Ip = MainSettings.IPServer; OnPropertyChanged("Ip");
                    Port = MainSettings.PortServer; OnPropertyChanged("Port");
                    Alis = MainSettings.AddresListIDServer; OnPropertyChanged("Alis");
                });
            }
            catch (Exception ex)
            {
                Log.Write(TypeLog.Error, ex.Message);
            }
          
        }
        public void SyncronaizeMainGui()
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    
                });
            }
            catch (Exception ex)
            {
                Log.Write(TypeLog.Error, ex.Message);
            }

        }

        public bool AcceptInject
        {
            get { return acceptInject; }
            set
            {
                acceptInject = value;
                MainSettings.StatusInject = value;
                if (value) ControllerAssistent.StartInjectionServer();
                else ControllerAssistent.StopInjectionServer();
            }
        }
        public bool StatusTBot
        {
            get { return statusTBot; }
            set
            {
                statusTBot = value;
                MainSettings.StatusTBot = value;
                if (value) ControllerAssistent.StartTelegramBot();
                else ControllerAssistent.StopTelegramBot();
            }
        }
        public bool StatusWebServers
        {
            get { return statusWebServers; }
            set
            {
                statusWebServers = value;
                MainSettings.StatusWebServer = value;
                if (value) ControllerAssistent.StartWebServer();
                else ControllerAssistent.StopWebServer();
            }
        }
        public string IpInject
        {
            get { return ipInject; }
            set
            {
                ipInject = value;
                MainSettings.IpInject = value;
            }
        }
        public string MainPref
        {
            get { return mainPref; }
            set
            {
                mainPref = value;
                MainSettings.MainPrefix = value;
            }
        }
        public string Apikey
        {
            get { return apikey; }
            set
            {
                apikey = value;
                MainSettings.APIKey = value;
            }
        }
      
        public string MainFold
        {
            get { return mainFold; }
            set
            {
                mainFold = value;
                MainSettings.MainFolder = value;
            }
        }
        public int Alis
        {
            get { return alis; }
            set
            {
                alis = value;
                MainSettings.AddresListIDServer = value;

            }
        }
        public int Bhaud
        {
            get { return bhaud; }
            set
            {
                bhaud = value;
                MainSettings.BaudRate = value;
                
            }
        }
        public int PortInject
        {
            get { return portInject; }
            set
            {
                portInject = value;
                MainSettings.PortInject = value;

            }
        }
        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                MainSettings.PortServer = value;

            }
        }
        public string Ip
        {
            get { return ip; }
            set
            {
                ip = value;
                MainSettings.IPServer = value;
                loadIps();
            }
        }
        public string Ssid
        {
            get { return ssid; }
            set
            {
                ssid = value;
                MainSettings.SSID = value;
            }
        }
        public string Passworld
        {
            get { return passworld; }
            set
            {
                passworld = value;
                MainSettings.PASSWORLD = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    
    public partial class MainWindow : Window
    {
        
        public static Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
        public static MainWindow MainWindow_Static;
        public static MainWindow_ViewModel date;
        public Mutex mut = new Mutex();
        public Mutex mutWeb = new Mutex();
        public MainWindow()
        {
            Windows.Code.CodePage.Init();
            InitializeComponent();


        }

        private string fierwarePath = string.Empty;
        private void OpenFierware_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Binary Files (*.bin)|*.bin";

            if (openFileDialog.ShowDialog() == true)
            {
                fierwarePath = openFileDialog.FileName;
            }
        }
        private async void StartUpdateFierware_Click(object sender, RoutedEventArgs e)
        {
            if (ArduinoFierware.SelectedIndex != -1 && fierwarePath.Any())
            {
                //ArduinoClient
                await Task.Run(async () =>
                {
                    await App.Current.Dispatcher.InvokeAsync(() => FierwareStatus.Text = "Updating");

                    await App.Current.Dispatcher.InvokeAsync(async () => await Task.Run(()=>FierwareStatus.Text =
                    ControllerAssistent.UpdateOTAArduino(fierwarePath, ArduinoFierware.SelectedItem as IInternetClient)));
                });

            }
            
            
        }

        private void TIDAdd_Click(object sender, RoutedEventArgs e)
        {
           if(long.TryParse(TIDAdd.Text, out var res))
            {
                MainSettings.WhiteListID.Add(res);
            }
        }
        private void TIDDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TIDList.SelectedIndex>=0)MainSettings.WhiteListID.RemoveAt(TIDList.SelectedIndex);
            
        }
        private void InjectDelete_Click(object sender, RoutedEventArgs e)
        {
            if (InjectItems.SelectedIndex >= 0) Account.RemoveID(InjectItems.SelectedIndex);

        }

        private void GoConstructor_Click(object sender, RoutedEventArgs e)
        {
            while (mainFrame.NavigationService.RemoveBackEntry() != null) ;
            mainFrame.Navigate(new Module.DesignerCode.DesignerCode());

        }
        private void GoScript_Click(object sender, RoutedEventArgs e)
        {
            while (mainFrame.NavigationService.RemoveBackEntry() != null) ;
            mainFrame.Navigate(new Windows.Code.MainPageCode());
        }
        public static Dictionary<string, MyAssistentDLL.Module.Codes.MenuArgumentItem> menu = new Dictionary<string, MyAssistentDLL.Module.Codes.MenuArgumentItem>()
        {
            {"Path",  new MyAsistent.Windows.Page_Add_PathModule()},
            {"Arduino",  new MyAsistent.Windows.ArduinoNonResult()},
            {"ArduinoReadDate",  new MyAsistent.Windows.ArduinoCommandCreate()},
            {"Voice",  new MyAsistent.Windows.AddVoiceCommandxaml()},
            {"Keyboard",  new MyAsistent.Windows.NewKeyboad()},
           // {"Voice",  new MyAsistent.Windows.AddVoiceCommandxaml()},
            
            
        };
        private void ChangeTimePicker_SaveTime(object sender, TimeSpan Old, TimeSpan New)
        {
            MainSettings.SaveCommandTime = New;MainSettings.Init();
        }
        private void ChangeTimePicker_WaitTime(object sender, TimeSpan Old, TimeSpan New)
        {
            if (New != MainSettings.WaitForConectionDevice)
            {
                MainSettings.WaitForConectionDevice = New; 
               ControllerAssistent.RestartInternetWorker();
            }
        }
        public void SyncronizateMainSeetings()
        {
            App.Current.Dispatcher.Invoke(() => {
                //Setings load
                this.Settings_VoiceLog.IsChecked = MainSettings.VoiceLog;
                this.Settings_VoiceMessage.IsChecked = MainSettings.VoiceMessage;
                try
                {
                    this.Seting_SetSpechCulture.SelectedItem = MainSettings.SpeechCulture;
                }
                catch (Exception ex)
                {
                    Log.Write(TypeLog.Error, ex.Message);
                }
                try
                {
                    this.Setting_SetSpeechVoice.SelectedItem = MainSettings.VoiceCulture;
                }
                catch (Exception ex)
                {
                    Log.Write(TypeLog.Error, ex.Message);
                }
                MainSettings.Init();
                SaveTime.Time = MainSettings.SaveCommandTime;
                WaitTime.Time = MainSettings.WaitForConectionDevice;
                ControllerAssistent.RestartInternetWorkerDispetcher(); 
            });
            

        }
        public async void InilizationApp()
        {

            MainWindow_Static = this;
            foreach (var item in MyAsistent.Lang.LanguageManager.Lang)
                this.Lang.Items.Add(item);
            this.Lang.SelectedIndex = 0;
            Lang.SelectionChanged += Lang_SelectionChanged;

            

            InjectItems.ItemsSource = Account.Accounts;
            SaveTime.Time = MainSettings.SaveCommandTime;
            WaitTime.Time = MainSettings.WaitForConectionDevice;
          
          

            ComSettings.ItemsSource = ControllerAssistent.GetAllCom();
            ComSettings.SelectionChanged += ComSettings_SelectionChanged;

            Log.SendMsgToConsoleEvent += Log_SendMsgToConsoleEvent;
            Log.SendMsgToConsoleWebServerEvent += Log_SendMsgToConsoleWebServerEvent;

            //load
            TIDList.ItemsSource = MainSettings.WhiteListID;



            foreach (var i in ControllerAssistent.AllVoiceCultureName())
                Setting_SetSpeechVoice.Items.Add(i.ToString());
            foreach (var i in ControllerAssistent.AllSoundCultureName())
                if (i.Culture.Name.Length != 0)
                    Seting_SetSpechCulture.Items.Add(i.Culture.Name);
            if (Seting_SetSpechCulture.Items.Count == 0) Log.Write(TypeLog.Error, "Не установлен не один пакет распознование голоса работа программы не возможна!");
            if (Setting_SetSpeechVoice.Items.Count == 0) Log.Write(TypeLog.Error, "Не установлен не один пакет воспроиведения голоса работа программы не возможна!");


            //Setings load
            this.Settings_VoiceLog.IsChecked = MainSettings.VoiceLog;
            this.Settings_VoiceMessage.IsChecked = MainSettings.VoiceMessage;




            //checkerror
            if (menu.Keys.Count != (ControllerAssistent.GetAllNameTypeArgument().Length))
                Log.Write(TypeLog.Warning, "Не все окна привязаны к типам ввода");
            if (MainSettings.CultureText.Count != Seting_SetSpechCulture.Items.Count)
            {
                Log.Write(TypeLog.Error, "Не все языки распознаны");
                Log.Write(TypeLog.Message, "Попытка исправить");
                try
                {
                    foreach (var i in Seting_SetSpechCulture.Items)
                        if (!MainSettings.CultureText.ContainsKey(i.ToString()))
                            MainSettings.CultureText.Add(i.ToString(), " ");
                    Log.Write(TypeLog.Message, "Успешно исправленно!");
                }
                catch (Exception)
                {
                    Log.Write(TypeLog.Error, "Невозможно исправить дальнейшая работа программы не возможна");

                }
            }


            listCultureText.ItemsSource = MainSettings.CultureText;
            ArduinoSocketUsersComboBox.SelectionChanged += ArduinoSocketUsersComboBox_SelectionChanged;

            try
            {
                this.Seting_SetSpechCulture.SelectedItem = MainSettings.SpeechCulture;
            }
            catch (Exception ex)
            {
                Log.Write(TypeLog.Error, ex.Message);
            }
            try
            {
                this.Setting_SetSpeechVoice.SelectedItem = MainSettings.VoiceCulture;
            }
            catch (Exception ex)
            {
                Log.Write(TypeLog.Error, ex.Message);
            }

           

            date = new MainWindow_ViewModel();
            DataContext = date;

            //Slider
            this.sliderSens.Value = MainSettings.Sensivity;
            this.sliderSens.ValueChanged += SliderSens_ValueChanged;


            ControllerAssistent.OnConnetctedDevicesInternet += ControllerAssistent_OnConnetctedDevicesInternet;

        }

        private void ControllerAssistent_OnConnetctedDevicesInternet(string Name, IInternetClient Client)
        {
            switch (Name)
            {
                case "Arduino":
                    ArduinoSocketUsersComboBox.Items.Add(Client);
                    break;
                case "PC":
                    PCClients.Items.Add(Client);
                    break;
            }
        }

        private void SliderSens_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ControllerAssistent.VoiceSensivity = sliderSens.Value;
        }

        private void Lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           var SelLang = Lang.SelectedItem as MyAsistent.Lang.CultureLang;
            App.LanguageManager.SelectedLanguage = SelLang.code;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

           

        }

        private void CallingAddMenu_Click(object sender, RoutedEventArgs e)
        {
           Windows.CreateCommandBox cte = new Windows.CreateCommandBox();
            cte.ShowDialog();
        }
        private void InjectAdd_Click(object sender, RoutedEventArgs e)
        {
            if (InjectLogin.Text.Length > 0 && InjectPass.Text.Length > 0)
                if (!Account.Add(InjectLogin.Text, InjectPass.Text))
                    MessageBox.Show("Такой пользователь уже есть в системе");
        }

        private Paragraph CreateParagraph(TypeLog type, string message)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run($"{DateTime.Now.ToLongDateString()} ({DateTime.Now.ToLongTimeString()}) | {message} | ") { Foreground = new SolidColorBrush(Colors.LightGreen) });
            switch (type)
            {
                case TypeLog.Message:
                    para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Green) }));
                    break;
                case TypeLog.Error:
                    para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Red) }));
                    break;
                case TypeLog.Warning:
                    para.Inlines.Add(new Bold(new Run(type.ToString()) { Foreground = new SolidColorBrush(Colors.Yellow) }));
                    break;

            }
            return para;
        }
        private async Task WriteLog(RichTextBox Console,TypeLog type, string message)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                var Text = CreateParagraph(type, message);
                try
                {
                    await App.Current.Dispatcher.BeginInvoke(new Action(() => Console.Document.Blocks.Add(Text)));

                }
                catch (Exception ex)
                {

                    Log.Write(TypeLog.Error, ex.Message);
                }
            });
           
        }
        private async void Log_SendMsgToConsoleWebServerEvent(TypeLog type, string message)
        {
            await WriteLog(LogWebServer, type, message);
        }

        private async void Log_SendMsgToConsoleEvent(TypeLog type, string message)
        {
            await WriteLog(OutputLog, type, message);
        }

        private void ArduinoSocketUsersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ArduinoSocketUsersComboBox.SelectedValue as IInternetClient;
            if (selected != null)
                ArduinoSocketUsersListBox.ItemsSource = ControllerAssistent.GetAllCommandArduino(selected);
            else
            {
                ArduinoSocketUsersListBox.ItemsSource = null;
                ArduinoSocketUsersListBox.Items.Clear();
            }
        }

        private void ComSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainSettings.COMname = ComSettings.SelectedItem.ToString();
            ControllerAssistent.RestartCom();
        }

        private void ListCultureText_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
            var item = ((KeyValuePair<string, string>)listCultureText.SelectedItem);

            var message = new MyAsistent.Module.MessageBoxes.GetString($"Set Elemet {item.Key}", "Elemets");
            message.ShowDialog();
            if (message.result != String.Empty)
            {
                MainSettings.CultureText[item.Key] = message.result;
                //listCultureText.ItemsSource = MainSettings.CultureText;
            }
        }

        private void Settings_VoiceMessage_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true) MainSettings.VoiceMessage = true;
            else if ((sender as CheckBox).IsChecked == false) MainSettings.VoiceMessage = false;
        }
        private void Settings_VoiceLog_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true) MainSettings.VoiceLog = true;
            else if ((sender as CheckBox).IsChecked == false) MainSettings.VoiceLog = false;
        }
        private void RestartServer_Click(object sender, RoutedEventArgs e)
        {
            ControllerAssistent.RestartInternetWorker();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
           
            MainSettings.Save();
        }
      
        private void TypePath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Coomand.ItemsSource = null;
            var obj = new Windows.AddItem((sender as ComboBox).SelectedItem.ToString());
            if (obj.tryInit)
                obj.Show();
            else Log.Write(TypeLog.Error, $"Не найдена страница {(sender as ComboBox).SelectedItem}");
            Coomand.ItemsSource = ControllerAssistent.GetCurrentLangCulture();
        }
      
        private void Setting_SetSpeechVoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ControllerAssistent.SetVoiceSpeech((sender as ComboBox).SelectedItem.ToString());
            MainSettings.VoiceCulture = (sender as ComboBox).SelectedItem.ToString();
        }
        private void Setting_Seting_SetSpechCulture(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Items.Count == 1) return;
            Coomand.ItemsSource = null;
            ControllerAssistent.ChangeCulture((sender as ComboBox).SelectedItem.ToString());
            Coomand.ItemsSource = ControllerAssistent.GetCurrentLangCulture();

        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (inputeText.Text != "")
                {
                    var res = MyAssistentDLL.Module.Cmd.Cmd.Push(inputeText.Text);
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Run($"<=| {inputeText.Text} |=>\n") { Foreground = new SolidColorBrush(Colors.Red) });
                    para.Inlines.Add(new Run($"{DateTime.Now.ToLongDateString()} |") { Foreground = new SolidColorBrush(Colors.LightGreen) });
                    switch (res.Item1)
                    {
                        case MyAssistentDLL.Module.Cmd.Cmd.TypeCmd.Successful:
                            para.Inlines.Add(new Run($" {res.Item2} | ") { Foreground = new SolidColorBrush(Colors.Green) });

                            break;
                        case MyAssistentDLL.Module.Cmd.Cmd.TypeCmd.Error:
                            para.Inlines.Add(new Run($" {res.Item2} | ") { Foreground = new SolidColorBrush(Colors.Red) });

                            break;

                    }
                    OutputLog.Document.Blocks.Add(para); 

                    inputeText.Text = "";
                }
            }

        }
    }
}
