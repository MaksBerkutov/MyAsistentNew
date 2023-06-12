using System.ComponentModel;
using UniversalConnector.ViewModels;
using Xamarin.Forms;

namespace UniversalConnector.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}