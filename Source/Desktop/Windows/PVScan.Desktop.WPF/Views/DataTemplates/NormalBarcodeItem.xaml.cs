using PVScan.Core.Models;
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

namespace PVScan.Desktop.WPF.Views.DataTemplates
{
    /// <summary>
    /// Interaction logic for NormalBarcodeItem.xaml
    /// </summary>
    public partial class NormalBarcodeItem : UserControl
    {
        public static readonly DependencyProperty FavoriteCommandProperty =
            DependencyProperty.Register(nameof(FavoriteCommand), typeof(ICommand), typeof(UserControl));

        public ICommand FavoriteCommand
        {
            get
            {
                return (ICommand)GetValue(FavoriteCommandProperty);
            }

            set
            {
                SetValue(FavoriteCommandProperty, value);
            }
        }

        public NormalBarcodeItem()
        {
            InitializeComponent();

            // Initialization, basically
            DataContextChanged += (_, _) =>
            {
                if (DataContext == null)
                {
                    return;
                }

                ToggleFavoriteOpacity();
            };
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            FavoriteCommand.Execute(DataContext);
        }

        private void ToggleFavoriteOpacity()
        {
            if ((DataContext as Barcode).Favorite)
            {
                FavoriteButtonIcon.Opacity = 1;
            }
            else
            {
                FavoriteButtonIcon.Opacity = 0.15;
            }
        }
    }
}
