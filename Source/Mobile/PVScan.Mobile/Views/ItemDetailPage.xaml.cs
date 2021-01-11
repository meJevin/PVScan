using PVScan.Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PVScan.Mobile.Views
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