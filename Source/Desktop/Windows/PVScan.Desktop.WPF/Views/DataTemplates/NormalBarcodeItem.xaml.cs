using PVScan.Core;
using PVScan.Core.Models;
using System;
using System.Collections.Generic;
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

namespace PVScan.Desktop.WPF.Views.DataTemplates
{
    /// <summary>
    /// Interaction logic for NormalBarcodeItem.xaml
    /// </summary>
    public partial class NormalBarcodeItem : UserControl
    {
        public static readonly DependencyProperty FavoriteCommandProperty =
            DependencyProperty.Register(nameof(FavoriteCommand), typeof(ICommand), typeof(UserControl));

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(UserControl), 
                new PropertyMetadata(false, IsEditingChanged));

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

        public bool IsEditing
        {
            get
            {
                return (bool)GetValue(IsEditingProperty);
            }
            set
            {
                SetValue(IsEditingProperty, value);
            }
        }

        private static async void IsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var barcodeItemControl = d as NormalBarcodeItem;
            var isEditing = (bool)e.NewValue;

            if (isEditing)
            {
                await barcodeItemControl.MakeEditable();
            }
            else
            {
                await barcodeItemControl.MakeNotEditable();
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

            _ = MakeNotEditable();
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

        public async Task MakeEditable()
        {
            //this.Background = new SolidColorBrush(Colors.Red
            _ = FavoriteButton.TranslateTo(38, 0, Animations.DefaultDuration);
            await SelectedIcon.TranslateTo(0, 0, Animations.DefaultDuration);
        }

        public async Task MakeNotEditable()
        {
            _ =FavoriteButton.TranslateTo(0, 0, Animations.DefaultDuration);
            await SelectedIcon.TranslateTo(38, 0, Animations.DefaultDuration);
            //this.Background = new SolidColorBrush(Colors.Blue);
        }
    }
}
