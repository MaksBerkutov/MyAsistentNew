using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MyAsistent.MyGUIElemet
{
    /// <summary>
    /// Логика взаимодействия для TimePicker.xaml
    /// </summary>
    public class ModelTimerPicker : INotifyPropertyChanged
    {

        private string h;
        private string m;
        private string s;

        public ModelTimerPicker()
        {
            OnPropertyChanged("H");
            OnPropertyChanged("M");
            OnPropertyChanged("S");
        }
        public TimeSpan Time
        {
            get
            {
                return time;
            }
            set
            {

                time = value;
                H = value.Hours.ToString();
                M = value.Minutes.ToString();
                S = value.Seconds.ToString();
            }
        }
        private TimeSpan time = new TimeSpan(0, 0, 0);

        public delegate void ChangeTimePicker(object sender, TimeSpan Old, TimeSpan New);
        public event ChangeTimePicker ChangeTimePickerhandler;
        public void startEvent()
        {
            if (h != null && m != null && s != null &&
                h.Length != 0 && m.Length != 0 && s.Length != 0)
                ChangeTimePickerhandler?.Invoke(this,
                    time,
                    time = new TimeSpan(
                        int.Parse(h),
                        int.Parse(m),
                        int.Parse(s)
                    ));
            
        }
        public string H
        {
            get { return h; }
            set
            {
                if (value.Length != 0 && int.TryParse(value, out int res) && res >= 0 && res <= 60) { h = value; OnPropertyChanged("H"); }

            }
        }
        public string M
        {
            get { return m; }
            set
            {
                if (value.Length != 0 && int.TryParse(value, out int res) && res >= 0 && res <= 60) 
                { m = value; OnPropertyChanged("M"); }
            }
        }
        public string S
        {
            get { return s; }
            set
            {
                if (value.Length != 0 && int.TryParse(value, out int res) && res >= 0 && res <= 60) { s = value; OnPropertyChanged("S"); }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
                startEvent();
            }
        }
    }
    public partial class TimePicker : UserControl
    {
        ModelTimerPicker obj;

        public TimePicker()
        {
            InitializeComponent();
            obj = new ModelTimerPicker();
            DataContext = obj;
            obj.ChangeTimePickerhandler += Obj_ChangeTimePickerhandler;
        }
        private void Obj_ChangeTimePickerhandler(object sender, TimeSpan Old, TimeSpan New)
        {
            this.ChangeTimePickerHandler?.Invoke(sender, Old, New);
        }
        public TimeSpan Time
        {
            get => obj.Time;
            set => obj.Time = value;
        }
        public event ModelTimerPicker.ChangeTimePicker ChangeTimePickerHandler;
    }
}
