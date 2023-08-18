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

namespace MyAsistent.Module.DesignerCode
{
    /// <summary>
    /// Логика взаимодействия для DesignerCode.xaml
    /// </summary>
    public class ControllerItem
    {
        RowDefinition GridCount;
        public MyItem.ICode Item;

        private bool isDragging = false;
        private Point StartPoint;

        public delegate void SwapElement(ControllerItem One, MyItem.ICode Two);
        public event SwapElement SwapedElement;
        public ControllerItem(RowDefinition gridCount, MyItem.ICode item)
        {
            GridCount = gridCount ?? throw new ArgumentNullException(nameof(gridCount));
            Item = item ?? throw new ArgumentNullException(nameof(item));


            Item.GetUserControl().PreviewMouseRightButtonDown += GridCount_PreviewMouseRightButtonDown;
            Item.GetUserControl().PreviewMouseMove += GridCount_PreviewMouseMove;
            Item.GetUserControl().Drop += GridCount_Drop;
            Item.GetUserControl().AllowDrop = true;
        }

        public void Swap(ref ControllerItem obj)
        {
            var save = this.Item;
            this.Item = obj.Item;
            obj.Item = save;
        }

        private void RemoveRow()
        {
            if (GridConroller.MainWin.Main.RowDefinitions.Contains(this.GridCount))
            {
                GridConroller.MainWin.Main.RowDefinitions.Remove(this.GridCount);
            }
        }
        private void RemoveItem()
        {
            if (GridConroller.MainWin.Main.Children.Contains(this.Item.GetUserControl()))
            {
                GridConroller.MainWin.Main.Children.Remove(this.Item.GetUserControl());
            }
        }
        public void Remove()
        {
            RemoveRow();
            RemoveItem();
        }

        public void UpdatePostionOnGrid()
        {
            Grid.SetRow(Item.GetUserControl(), GridConroller.MainWin.Main.RowDefinitions.IndexOf(GridCount));
        }

        private void GridCount_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            StartPoint = e.GetPosition(null);
        }
        private void GridCount_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var itemUserControll = Item.GetUserControl();

                Point mousePos = e.GetPosition(null);
                Vector diff = StartPoint - mousePos;

                DataObject data = new DataObject(typeof(UserControl), itemUserControll);
                DragDrop.DoDragDrop(itemUserControll, data, DragDropEffects.Move);

                isDragging = false;
            }
        }
        private void Swap(UserControl UserControls)
        {
            var oldRow = Grid.GetRow(UserControls);
            Grid.SetRow(UserControls, GridConroller.MainWin.Main.RowDefinitions.IndexOf(GridCount));
            Grid.SetRow(this.Item.GetUserControl(), oldRow);
        }
        private void GridCount_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(UserControl)))
            {
                MyItem.ICode draggedElement = e.Data.GetData(typeof(UserControl)) as MyItem.ICode;

                if (draggedElement != null)
                {
                    Swap(draggedElement.GetUserControl());
                    this.SwapedElement.Invoke(this, draggedElement);
                }
            }
        }


    }

    public class GridConroller
    {
        DesignerCode MainWindow;
        List<ControllerItem> items;

        public static DesignerCode MainWin { get; private set; }
        private Border CreateBorder()
        {
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            return border;
        }
        private RowDefinition CreateRowDef()
        {
            RowDefinition row = new RowDefinition();
            MainWindow.Main.RowDefinitions.Add(row);
            Grid.SetRow(CreateBorder(), MainWindow.Main.RowDefinitions.IndexOf(row));
            return row;

        }
        public GridConroller(DesignerCode mainWindow)
        {
            this.MainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            MainWin = mainWindow;
            items = new List<ControllerItem>();

        }
        public void Add(MyItem.ICode item)
        {
            var rowDef = CreateRowDef();
            var indexRow = MainWindow.Main.RowDefinitions.IndexOf(rowDef);
            Grid.SetRow(item.GetUserControl(), indexRow);
            MainWindow.Main.Children.Add(item.GetUserControl());
            item.OnDelete += Item_OnDelete;

            var controlItem = new ControllerItem(rowDef, item);
            controlItem.SwapedElement += ControlItem_SwapedElement;
            items.Add(controlItem);


        }

        private void Item_OnDelete(MyItem.ICode sender)
        {
            int FindedIndex = items.FindIndex(x => x.Item.Equals(sender));
            items[FindedIndex].Remove();
            items.RemoveAt(FindedIndex);

            for (int i = FindedIndex; i < items.Count; i++)
                items[i].UpdatePostionOnGrid();

        }

        private void ControlItem_SwapedElement(ControllerItem One, MyItem.ICode Two)
        {
            var elementTwo = items.Find(x => x.Item.Equals(Two));
            if (elementTwo != null)
                elementTwo.Swap(ref One);

        }

        public string GetCode()
        {
            string code = "using System;\n" +
                "class Program{\n" +
                "   public static void Main(){\n";
            foreach (var item in items)
            {
                code += item.Item.GetCode() + "\n";
            }
            code += "  }\n" +
            "}";
            return code;
        }
    }
    public partial class DesignerCode : Page
    {
        GridConroller MainController;
        Dictionary<string, MyItem.ICode> userControls = new Dictionary<string, MyItem.ICode>
        {
            { "pack://application:,,,/Image/Arduino.png",new MyItem.Arduino()},
            { "pack://application:,,,/Image/Vosie.png",new MyItem.Vosie()},

        };
        public DesignerCode()
        {
            InitializeComponent();
            MainController = new GridConroller(this);
        }

        private void _MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var result = userControls[(sender as Image).Source.ToString()].GetNewElement();
            MainController.Add(result);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = MainController.GetCode();
        }
    }
}
