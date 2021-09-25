using PVScan.Core.Models;
using PVScan.Desktop.WPF.ViewModels;
using PVScan.Desktop.WPF.ViewModels.Messages.Barcodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            MessagingCenter.Subscribe<HistoryPageViewModel, BarcodeDeletedMessage>(this,
                nameof(BarcodeDeletedMessage),
                (sender, args) => 
                { 
                    if (VM.SelectedBarcode == args.DeletedBarcode)
                    {
                        VM.SelectedBarcode = null;
                        Closed?.Invoke(this, new EventArgs());
                    }
                });

            VM.PropertyChanged += VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VM.SelectedBarcodeHasLocation) || 
                e.PropertyName == nameof(VM.SelectedBarcode))
            {
                if (VM.SelectedBarcodeHasLocation)
                {
                    ShowNormalLocationMenu();
                }
                else
                {
                    ShowNoLocationMenu();
                }
            }
        }

        // Shows an exlamaition point and no location label
        private void ShowNoLocationMenu()
        {
            LocationNotAvailableIcon.Opacity = 1;
            LocationNotAvailableLabel.Opacity = 1;
            LocationField.Opacity = 0;
            LocationNotAvailableOverlay.IsHitTestVisible = true;
        }

        // Shows coordinates
        private void ShowNormalLocationMenu()
        {
            LocationNotAvailableOverlay.IsHitTestVisible = false;
            LocationNotAvailableIcon.Opacity = 0;
            LocationNotAvailableLabel.Opacity = 0;
            LocationField.Opacity = 1;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, new EventArgs());
            VM.CloseCommand.Execute(null);
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
