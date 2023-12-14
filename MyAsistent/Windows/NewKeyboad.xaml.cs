using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyAssistentDLL.Module.Codes;
using MyAssistentDLL.Logs;


namespace MyAsistent.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewKeyboad.xaml
    /// </summary>
    public class Vertical
    {
        Horizontal CurrentMoveHorizontal;
        Vertical NextVertical;
        int VerticalId;
        int HorizontalID;
        public Vertical(int vid,int hid)
        {
            VerticalId = vid;
            HorizontalID = hid;
            CurrentMoveHorizontal = new Horizontal(vid);
            NextVertical = null;
        }
        public void addVertical(int vid,int hid)
        {
            if (NextVertical == null)
                NextVertical = new Vertical(vid, hid);
            else NextVertical.addVertical(vid, hid);
        }
        public void AddHorizontal(int id)
        {
            if (HorizontalID == id)
                CurrentMoveHorizontal.add();
            else NextVertical.AddHorizontal(id);
        }
        private string getAllStringsHorizontal()
        {
            if (NextVertical == null)
                return CurrentMoveHorizontal.GetKeyboard();
            else
            {
                return NextVertical.getAllStringsHorizontal() + "=" + CurrentMoveHorizontal.GetKeyboard() ;
            }
        }
        public List<string> GetCombi()
        {
          var list = getAllStringsHorizontal().Split('=').ToList();
          var clearList = new List<string>();   
            foreach(var item in list)
                if(item.Length > 0)
                    clearList.Add(item);
            return clearList;
        }

    }
    public class Horizontal
    {
        int VerticalId;
        List<TextBox> TextBoxses;
        public static TextBox ForRead;
        public delegate void DelHorizontal(int id);
        public static event DelHorizontal DelHorizontalEvent;
        public Horizontal(int vid)
        {
            VerticalId = vid;
            TextBoxses = new List<TextBox>();
        }
        public string GetKeyboard()
        {
            string ret = "";
            foreach (var item in TextBoxses)
                if (item == TextBoxses[TextBoxses.Count - 1]) {
                    if (item.Text.Length != 0 && item != ForRead)
                        ret += item.Text;
                }
                else if (item.Text.Length != 0 && item != ForRead)
                    ret += item.Text+" ";
            return ret;

        }
        public void add()
        {
            TextBoxses.Add(new TextBox());
            TextBoxses[TextBoxses.Count - 1].Height = GridsCustom.fixedSizeX;
            TextBoxses[TextBoxses.Count - 1].Width = GridsCustom.fixedSizeY;
            TextBoxses[TextBoxses.Count - 1].IsReadOnly = true;
            TextBoxses[TextBoxses.Count - 1].Name = "T"+TextBoxses.Count.ToString();
            TextBoxses[TextBoxses.Count - 1].MouseDoubleClick += Horizontal_MouseDoubleClick;
            TextBoxses[TextBoxses.Count - 1].PreviewMouseRightButtonUp += Horizontal_PreviewMouseLeftButtonUp;
            //add logic to double click
            Canvas.SetTop(TextBoxses[TextBoxses.Count - 1],VerticalId);
            if (TextBoxses.Count == 1)
                Canvas.SetLeft(TextBoxses[TextBoxses.Count - 1], 0);
            else
                Canvas.SetLeft(TextBoxses[TextBoxses.Count - 1], Canvas.GetLeft(TextBoxses[TextBoxses.Count - 2])+ GridsCustom.fixedSizeY);
            GridsCustom.Main.Children.Add(TextBoxses[TextBoxses.Count - 1]);


        }

        private void Horizontal_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = ((TextBox)sender);
           
            if(TextBoxses.Count>1)
                for (int i = int.Parse(item.Name.Remove(0, 1)); i < TextBoxses.Count; i++)
                {
                    if (i <= 0) i = 1;
                    item.Name = "DeletedElemetHorizontal";
                    GridsCustom.Main.Children.Remove(TextBoxses[i]);
                    TextBoxses[i].Name = "T" + (i-1).ToString();
                    Canvas.SetLeft(TextBoxses[i], Canvas.GetLeft(TextBoxses[i]) - GridsCustom.fixedSizeY);
                    GridsCustom.Main.Children.Add(TextBoxses[i]);
                }
            GridsCustom.Main.Children.Remove(item);
            TextBoxses.Remove(item);
            DelHorizontalEvent?.Invoke(VerticalId);
        }

        
       

        private void Horizontal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((TextBox)sender);
            item.Text = "Read";
            ForRead = item;
        }
    }
    public class GridsCustom
    {
        Vertical StartVertical;
        public static readonly int fixedSizeX = 20, fixedSizeY = 50;
        Button VerticalAdder;//Vertical
        List<Button> HorizontalAdder;
        public static Canvas Main;
        public void AddHorizontal()
        {
            HorizontalAdder.Add(new Button());

            HorizontalAdder[HorizontalAdder.Count-1].Height = fixedSizeX;
            HorizontalAdder[HorizontalAdder.Count-1].Width = fixedSizeY;
            Canvas.SetLeft(HorizontalAdder[HorizontalAdder.Count - 1], 0);
            if (HorizontalAdder.Count == 1)
            {
                Canvas.SetTop(HorizontalAdder[HorizontalAdder.Count - 1], 0);


            }
            else
            {
                Canvas.SetTop(HorizontalAdder[HorizontalAdder.Count - 1], Canvas.GetTop(HorizontalAdder[HorizontalAdder.Count - 2])+ fixedSizeX);
            }  
            //else
            //    HorizontalAdder[HorizontalAdder.Count - 1].Margin = new Thickness(0, HorizontalAdder[HorizontalAdder.Count - 2].Margin.Top+fixedSizeY, 0, 0);
            HorizontalAdder[HorizontalAdder.Count-1].Content = "Row";
            HorizontalAdder[HorizontalAdder.Count-1].Name ="B" + HorizontalAdder.Count.ToString();
            HorizontalAdder[HorizontalAdder.Count-1].Uid ="B" + HorizontalAdder.Count.ToString();
            HorizontalAdder[HorizontalAdder.Count-1].Click += HorizontalAdder_Click;
           
            Main.Children.Add(HorizontalAdder[HorizontalAdder.Count - 1]);
      
        }
        public GridsCustom(Canvas m)
        {
            m.KeyUp += M_KeyUp;
            m.MouseDown += M_MouseDown;
            Horizontal.DelHorizontalEvent += Horizontal_DelHorizontalEvent;
            Main = m;
            HorizontalAdder = new List<Button>();
            AddHorizontal();
            StartVertical = new Vertical(0,HorizontalAdder.Count);
            //Build Vertical
            VerticalAdder = new Button();
            VerticalAdder.Height = fixedSizeX;
            VerticalAdder.Width = fixedSizeY;
            //VerticalAdder.Margin = new Thickness(0, fixedSizeY, 0, 0);
            VerticalAdder.Click += VerticalAdder_Click;
            VerticalAdder.Content = "Col";
            Canvas.SetLeft(VerticalAdder, 0);
            Canvas.SetTop(VerticalAdder, fixedSizeX);
            Main.Children.Add(VerticalAdder);




        }

        private void Horizontal_DelHorizontalEvent(int id)
        {
            try
            {
                id = id / fixedSizeX;
                Canvas.SetLeft(HorizontalAdder[id], Canvas.GetLeft(HorizontalAdder[id]) - GridsCustom.fixedSizeY);
            }
            catch (Exception ex)
            {

                Log.Write(TypeLog.Error, ex.Message);
            }


        }

        public string[] GetResult()
        {
            return StartVertical.GetCombi().ToArray();
        }

        private void M_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Horizontal.ForRead != null)
            {
                Horizontal.ForRead.Text = "";
                Horizontal.ForRead = null;
            }
        }

        private void M_KeyUp(object sender, KeyEventArgs e)
        {
            if(Horizontal.ForRead != null)
            {
                Horizontal.ForRead.Text = e.Key.ToString();
                Horizontal.ForRead = null;
            }
        }

        private void VerticalAdder_Click(object sender, RoutedEventArgs e)
        {
            int vid = System.Convert.ToInt32(Canvas.GetTop(VerticalAdder));
            AddHorizontal();
            Canvas.SetTop(VerticalAdder, Canvas.GetTop(VerticalAdder) + fixedSizeX);
            StartVertical.addVertical(vid, HorizontalAdder.Count);
        }

        private void HorizontalAdder_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender);
            StartVertical.AddHorizontal(int.Parse(item.Name.Remove(0,1)));
            Canvas.SetLeft(item, Canvas.GetLeft(item) + GridsCustom.fixedSizeY);
        }
        public void RemoveMe()
        {
            Main.Children.Clear();

        }
    }
    public partial class NewKeyboad : Page, MenuArgumentItem
    {

        private (TypeArgumend, string[], bool local)? result = null;
        public (TypeArgumend, string[], bool local)? Result => result;
        public void clear()
        {
            result = null;
            worker.RemoveMe();
            worker = new GridsCustom(Main);
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            
            result = (TypeArgumend.Keyboard, worker.GetResult(), locals.IsChecked.Value);
            OnClose.Invoke(this);
        }

        public event Close OnClose;
        GridsCustom worker;
        public NewKeyboad()
        {
            InitializeComponent();
            worker = new GridsCustom(Main);
        }
    }
}
