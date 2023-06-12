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
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace ConvertsMY
{
    class Converter
    {

        private static string UP = "!\"№;%:?*()_+/,";
      
        private static string DOWN = "1234567890-=\\.";
        public static bool to_convert(char p)
        {
            foreach (var i in UP)
                if (i == p)
                    return true;
            foreach (var i in DOWN)
                if (i == p)
                    return true;
            return false;

        }
        public static char toUp(char p)
        {
            for (int i = 0; i < DOWN.Length; i++)
                if (DOWN[i] == p) return UP[i];
            return p;
        }
        public static char toDown(char p)
        {
            for (int i = 0; i < UP.Length; i++)
                if (UP[i] == p) return DOWN[i];
            return p;
        }
    }
}
namespace MyAsistent.Properties
{
    public class KeyboardEmu
    {
        public List<Key> keys;
        public KeyboardEmu() => keys = new List<Key>();
        public KeyboardEmu(List<Key> Keys) => keys = Keys;
        public override string ToString()=>String.Join("@",keys.ToArray());

        public static bool tryParse(string str, out KeyboardEmu obj)
        {
            obj = new KeyboardEmu();
            try
            {
                
                var items = str.Split('@');
                foreach (var item in items)
                    obj.keys.Add((Key)(Enum.Parse(typeof(Key),item)));
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для PageKeyboardEmu.xaml
    /// </summary>
    public partial class PageKeyboardEmu : Page
    {
        bool focus = true;
        public PageKeyboardEmu()
        {
            InitializeComponent();
        }
        bool tabOn = false;
        public static MyAsistent.Properties.KeyboardEmu keys = new MyAsistent.Properties.KeyboardEmu();
        public string[] RUS = new string[]
        {
            "ё1234567890-=",
            "йцукенгшщзхъ",
            "фывапролджэ\\",
            "ячсмитьбю."
        };
        public string[] EN = new string[]
       {
            "`1234567890-=",
            "qwertyuiop[]",
            "asdfghjkl;'\\",
            "zxcvbnm,./"
       };

        struct SaveDateBut
        {
            public struct Cord
            {
                public int x;
                public int y;

                public Cord(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }
            public Button But;
            public Cord crd;
            public static List<int> StartIndex = new List<int> { 0, 1, 2, 2 };
            public SaveDateBut(Button but, Cord crd)
            {
                But = but;
                this.crd = crd;
            }
        }

        List<SaveDateBut> but = new List<SaveDateBut>();
        void Generate()
        {
            if (Lang == false)
            {
                for (int i = 0; i < RUS.Length; i++)
                {
                    for (int j = SaveDateBut.StartIndex[i], j1 = 0; j1 < RUS[i].Length; j++, j1++)
                    {
                        but.Add(new SaveDateBut(new Button(), new SaveDateBut.Cord(i, j)));

                        but[but.Count - 1].But.Content = EN[i][j1];
                        but[but.Count - 1].But.Margin = new Thickness(5, 5, 5, 5);
                        Grid.SetColumn(but[but.Count - 1].But, j);
                        Grid.SetRow(but[but.Count - 1].But, i);
                        Keyboard.Children.Add(but[but.Count - 1].But);
                    }
                }
            }

        }

        bool Lang = false;
        private string PressKey(Key k, Color clr)
        {
            KeyConverter kc = new KeyConverter();
            var str = kc.ConvertToString(k);
            if (Lang == false)
            {
                bool exit = true;
                for (int i = 0, alllen = 0; i < EN.Length && exit; i++)
                    for (int j = 0; j < EN[i].Length && exit; j++, alllen++)
                        if (EN[i][j] == str.ToLower()[0] && str.Length == 1)
                        {
                            but[alllen].But.Background = new SolidColorBrush(clr);
                            if (but[alllen].But.Content.ToString() == but[alllen].But.Content.ToString().ToLower()) str = RUS[i][j].ToString();
                            else str = RUS[i][j].ToString().ToUpper();
                            exit = false;
                        }
                        else
                            switch (str)
                            {
                                case "Oem3"://`
                                    but[0].But.Background = new SolidColorBrush(clr);
                                    if (but[alllen].But.Content.ToString() == but[alllen].But.Content.ToString().ToLower()) str = "ё";
                                    else str = "Ё";
                                    exit = false;
                                    break;
                                case "OemMinus"://-
                                    but[11].But.Background = new SolidColorBrush(clr);
                                    str = "-"; exit = false;
                                    break;
                                case "OemPlus"://=
                                    but[12].But.Background = new SolidColorBrush(clr);
                                    str = "+"; exit = false;
                                    break;
                                case "OemOpenBrackets"://[
                                    but[23].But.Background = new SolidColorBrush(clr);
                                    str = "х"; exit = false;
                                    break;
                                case "Oem6"://]
                                    but[24].But.Background = new SolidColorBrush(clr);
                                    str = "ъ"; exit = false;
                                    break;
                                case "Oem1":// ;
                                    but[34].But.Background = new SolidColorBrush(clr);
                                    str = "ж"; exit = false;
                                    break;
                                case "OemQuotes"://'
                                    but[35].But.Background = new SolidColorBrush(clr);
                                    str = "э"; exit = false;
                                    break;
                                case "Oem5":// \
                                    but[36].But.Background = new SolidColorBrush(clr);
                                    str = "\\"; exit = false;
                                    break;
                                case "OemComma":// ,
                                    but[44].But.Background = new SolidColorBrush(clr);
                                    str = "б"; exit = false;
                                    break;
                                case "OemPeriod":// .
                                    but[45].But.Background = new SolidColorBrush(clr);
                                    str = "ю"; exit = false;
                                    break;
                                case "OemQuestion":// /
                                    but[46].But.Background = new SolidColorBrush(clr);
                                    if (but[alllen].But.Content.ToString() == but[alllen].But.Content.ToString().ToLower()) str = ".";
                                    else str = ","; exit = false;
                                    break;


                            }

            }

            return str;




        }
        void ToUP()
        {
            foreach (var i in but)
                if (ConvertsMY.Converter.to_convert(i.But.Content.ToString()[0]))
                    i.But.Content = ConvertsMY.Converter.toUp(i.But.Content.ToString()[0]);
                else
                    i.But.Content = i.But.Content.ToString().ToUpper();
        }
        void ToDown()
        {
            foreach (var i in but)
                if (ConvertsMY.Converter.to_convert(i.But.Content.ToString()[0]))
                    i.But.Content = ConvertsMY.Converter.toDown(i.But.Content.ToString()[0]);
                else
                    i.But.Content = i.But.Content.ToString().ToLower();

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (focus == false) return;
            switch (e.Key)
            {
                case Key.Enter:
                    Enter.Background  = new SolidColorBrush(Colors.Red);
                    break;
                case Key.LeftCtrl:
                    CtrlL.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.RightCtrl:
                    CtrlR.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.LWin:
                    WinL.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.RWin:
                    WinR.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.LeftAlt:
                    AltL.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.RightAlt:
                    AltR.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.Tab:
                    tabOn = !tabOn;
                    if (tabOn)
                    {
                        ToUP();
                        Tab.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        ToDown();
                        Tab.Background = new SolidColorBrush(Colors.LightGray);
                    }

                    break;
                case Key.LeftShift:
                    if (tabOn) { ShiftL.Background = new SolidColorBrush(Colors.Green); ToDown(); }
                    else { ShiftL.Background = new SolidColorBrush(Colors.Green); ToUP(); }
                    break;
                case Key.Space:
                    Space.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Key.RightShift:
                    if (tabOn) { ShiftP.Background = new SolidColorBrush(Colors.Green); ToDown(); }
                    else { ShiftP.Background = new SolidColorBrush(Colors.Green); ToUP(); }
                    break;
                default:
                   
                    PressKey(e.Key, Color.FromRgb(220, 20, 60));
                    break;
            }
            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (focus == false) return;

            switch (e.Key)
            {
                case Key.Enter:
                    Enter.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.LeftCtrl:
                    CtrlL.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.RightCtrl:
                    CtrlR.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.LWin:
                    WinL.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.RWin:
                    WinR.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.LeftAlt:
                    AltL.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.RightAlt:
                    AltR.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case Key.Tab:


                    break;
                case Key.LeftShift:
                    if (tabOn) { ShiftL.Background = new SolidColorBrush(Colors.LightGray); ToUP(); }
                    else { ShiftL.Background = new SolidColorBrush(Colors.LightGray); ToDown(); }
                    break;
                case Key.RightShift:
                    if (tabOn) { ShiftP.Background = new SolidColorBrush(Colors.LightGray); ToUP(); }
                    else { ShiftP.Background = new SolidColorBrush(Colors.LightGray); ToDown(); }
                    break;
                case Key.Space:
                    Space.Background = new SolidColorBrush(Colors.LightGray);
                    
                    break;
                default:
                    PressKey(e.Key, Color.FromRgb(211, 211, 211));
                    break;
                   
            }
            keys.keys.Insert(0,e.Key);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Generate();
            keys.keys.Clear();
            Try = false;
            OutKey.ItemsSource = keys.keys;
        }
        public static bool Try = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Codes.Code_Saver.add((Command.Text,MainSettings.SpeechCulture), (Codes.TypeArgumend.Keyboard,new string[] { keys.ToString() }));
            Try = true;this.Content = null;
        }

        private void Command_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Command.IsFocused) focus = false;
            else focus = true;
        }

        private void Command_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Command.IsFocused) focus = false;
            else focus = true;
        }

        private void OutKey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var i = OutKey.SelectedIndex;
            OutKey.ItemsSource = null;
            keys.keys.RemoveAt(i);
            OutKey.ItemsSource = keys.keys;
        }
    }
}
