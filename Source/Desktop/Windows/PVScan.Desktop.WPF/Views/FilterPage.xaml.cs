using PVScan.Core;
using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for FilterPage.xaml
    /// </summary>
    public partial class FilterPage : ContentControl
    {
        public event EventHandler Closed;

        FilterPageViewModel VM;

        public FilterPage()
        {
            InitializeComponent();

            VM = DataContext as FilterPageViewModel;

            AvailableBarcodeFormatsListView.SelectionChanged += BarcodeFormatListView_SelectionChanged;
            AvailableLastTimeSpansListView.SelectionChanged += LastTimeListView_SelectionChanged;

            VM.PropertyChanged += VM_PropertyChanged;

            _ = ShowLastDatePanel(TimeSpan.Zero);
        }

        private async void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.DateFilterTypeIndex))
            {
                if (VM.DateFilterTypeIndex == 0)
                {
                    await ShowLastDatePanel(Animations.DefaultDuration);
                }
                else if (VM.DateFilterTypeIndex == 1)
                {
                    await ShowDateRangePanel(Animations.DefaultDuration);
                }
            }
        }

        private void BarcodeFormatListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.ToggleApplyFilterEnabled();
        }

        private void LastTimeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.ToggleApplyFilterEnabled();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
        }

        private async Task ShowLastDatePanel(TimeSpan duration)
        {
            _ = DateTypeIndicator.TranslateTo(0, 0, duration);

            _ = DateLastButtonLabel.FadeTo(1, duration);
            _ = DateRangeButtonLabel.FadeTo(0.35, duration);
            _ = DateRangePanel.FadeTo(0, duration);
            DateRangePanel.IsHitTestVisible = false;
            DateLastPanel.IsHitTestVisible = true;
            await DateLastPanel.FadeTo(1, duration);
        } 

        private async Task ShowDateRangePanel(TimeSpan duration)
        {
            _ = DateTypeIndicator.TranslateTo(DateTypeIndicator.ActualWidth, 0, duration);

            _ = DateRangeButtonLabel.FadeTo(1, duration);
            _ = DateLastButtonLabel.FadeTo(0.35, duration);
            _ = DateLastPanel.FadeTo(0, duration);
            DateLastPanel.IsHitTestVisible = false;
            DateRangePanel.IsHitTestVisible = true;
            await DateRangePanel.FadeTo(1, duration);
        }

        private void DateTypeIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (VM.DateFilterTypeIndex == 1)
            {
                _ = DateTypeIndicator.TranslateTo(DateTypeIndicator.ActualWidth, 0, TimeSpan.Zero);
            }
        }
    }
}
