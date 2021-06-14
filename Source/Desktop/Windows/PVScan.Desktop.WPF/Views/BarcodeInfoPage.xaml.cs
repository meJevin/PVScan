using PVScan.Core.Models;
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
    /// Interaction logic for BarcodeInfoPage.xaml
    /// </summary>
    public partial class BarcodeInfoPage : ContentControl
    {
        public event EventHandler Closed;

        public static readonly DependencyProperty ShowOnMapCommandProperty =
            DependencyProperty.Register(nameof(ShowOnMapCommand), typeof(ICommand), typeof(ContentControl));
        public ICommand ShowOnMapCommand
        {
            get
            {
                return (ICommand)GetValue(ShowOnMapCommandProperty);
            }

            set
            {
                SetValue(ShowOnMapCommandProperty, value);
            }
        }

        public static readonly DependencyProperty ShowInListCommandProperty =
            DependencyProperty.Register(nameof(ShowInListCommand), typeof(ICommand), typeof(ContentControl));
        public ICommand ShowInListCommand
        {
            get
            {
                return (ICommand)GetValue(ShowInListCommandProperty);
            }

            set
            {
                SetValue(ShowInListCommandProperty, value);
            }
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(ContentControl));
        public ICommand DeleteCommand
        {
            get
            {
                return (ICommand)GetValue(DeleteCommandProperty);
            }

            set
            {
                SetValue(DeleteCommandProperty, value);
            }
        }

        BarcodeInfoPageViewModel VM;

        public BarcodeInfoPage()
        {
            InitializeComponent();

            VM = DataContext as BarcodeInfoPageViewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
        }

        private void ShowOnMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.SelectedBarcode == null)
            {
                return;
            }

            ShowOnMapCommand.Execute(VM.SelectedBarcode);
        }

        private void ShowInListButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.SelectedBarcode == null)
            {
                return;
            }

            ShowInListCommand.Execute(VM.SelectedBarcode);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (VM.SelectedBarcode == null)
            {
                return;
            }

            DeleteCommand.Execute(VM.SelectedBarcode);

            Closed?.Invoke(this, new EventArgs());
        }
    }
}
