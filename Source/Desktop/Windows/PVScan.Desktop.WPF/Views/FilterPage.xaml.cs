using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for FilterPage.xaml
    /// </summary>
    public partial class FilterPage : ContentControl
    {
        FilterPageViewModel VM;

        public FilterPage()
        {
            InitializeComponent();

            VM = DataContext as FilterPageViewModel;

            AvailableBarcodeFormatsListView.SelectionChanged += BarcodeFormatListView_SelectionChanged;
            AvailableLastTimeSpansListView.SelectionChanged += LastTimeListView_SelectionChanged;
        }

        private void BarcodeFormatListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.ToggleApplyFilterEnabled();
        }

        private void LastTimeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.ToggleApplyFilterEnabled();
        }
    }
}
