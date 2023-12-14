using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyAsistent.Module.DesignerCode.MyItem
{
    public interface ICode
    {
        string GetCode();
        UserControl GetUserControl();
        ICode GetNewElement();
        void Load(object[] arg);
        object[] Save();
        event Delete OnDelete;
    }
    public delegate void Delete(ICode sender);

}
