using System;
using System.Collections.Generic;
using System.ComponentModel;
using UniversalConnector.Models;
using UniversalConnector.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UniversalConnector.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}