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
    /// Interaction logic for SortingPage.xaml
    /// </summary>
    public partial class SortingPage : ContentControl
    {
        public event EventHandler Closed;

        SortingPageViewModel VM;

        public SortingPage()
        {
            InitializeComponent();

            VM = DataContext as SortingPageViewModel;
        }

        private void AvailiableSortingFieldsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM == null)
            {
                return;
            }

            VM.SortingFieldSelectedCommand.Execute(AvailiableSortingFieldsListView.SelectedItem);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
        }
    }
}
