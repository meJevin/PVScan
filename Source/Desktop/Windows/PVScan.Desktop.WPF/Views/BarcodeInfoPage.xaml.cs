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
        //public static readonly DependencyProperty SelectedBarcodeProperty =
        //    DependencyProperty.Register("s", typeof(Barcode), typeof(BarcodeInfoPage),
        //        new PropertyMetadata(null, SelectedBarcodePropertyChanged));

        //public Barcode SelectedBarcode
        //{
        //    get
        //    {
        //        return (Barcode)GetValue(SelectedBarcodeProperty);
        //    }

        //    set
        //    {
        //        SetValue(SelectedBarcodeProperty, value);
        //    }
        //}

        public BarcodeInfoPage()
        {
            InitializeComponent();
        }

        //private static void SelectedBarcodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var VM = ((BarcodeInfoPage)d).DataContext as BarcodeInfoPageViewModel;

        //    VM.SelectedBarcode = e.NewValue as Barcode;
        //}
    }
}
