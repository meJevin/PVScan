using PVScan.Core;
using PVScan.Desktop.WPF.Services.Interfaces;
using PVScan.Desktop.WPF.ViewModels.Messages.Popups;
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

namespace PVScan.Desktop.WPF.Views.Popups
{
    public class TextMessagePopupResult
    {
    }

    public class TextMessagePopupArgs
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Interaction logic for NoLocationAvailablePopup.xaml
    /// </summary>
    public partial class TextMessagePopup : UserControl, 
        IPopup<TextMessagePopupArgs, TextMessagePopupResult>
    {
        MainWindow MainWindow;
        TextMessagePopupResult Result;
        TextMessagePopupArgs Args;

        bool Dismissed;

        public TextMessagePopup()
        {
            InitializeComponent();

            MainWindow = Application.Current.MainWindow as MainWindow;

            MessagingCenter.Subscribe<MainWindow, PopupDismissedViaOverlayMessage>(
                this, nameof(PopupDismissedViaOverlayMessage), (sender, args) => 
                {
                    Dismissed = true;
                });

            _ = AnimateOut(TimeSpan.Zero);
        }

        public async Task<TextMessagePopupResult> ShowPopup(TextMessagePopupArgs args)
        {
            Args = args;

            Prepare();

            _ = AnimateMainWindowIn(Animations.DefaultDuration);
            await AnimateIn(Animations.DefaultDuration);

            await WaitOrDismiss();

            _ = AnimateOut(Animations.DefaultDuration);
            await AnimateMainWindowOut(Animations.DefaultDuration);

            return Result;
        }

        private void Prepare()
        {
            Dismissed = false;
            Result = new TextMessagePopupResult();
            MessageTextBlock.Text = Args.Message;
        }

        private async Task<TextMessagePopupResult> WaitOrDismiss()
        {
            var delayTask = Task.Delay(1500);
            while (!Dismissed && !delayTask.IsCompleted)
            {
                await Task.Delay(5);
            }

            return Result;
        }

        private async Task AnimateIn(TimeSpan duration)
        {
            await this.FadeTo(1, duration);
        }

        private async Task AnimateOut(TimeSpan duration)
        {
            await this.FadeTo(0, duration);
        }

        private async Task AnimateMainWindowIn(TimeSpan duration)
        {
            MainWindow.PopupContainer.IsHitTestVisible = true;
            MainWindow.PopupOverlay.IsHitTestVisible = true;
            MainWindow.PopupContainer.Children.Add(this);
            await MainWindow.PopupOverlay.FadeTo(0.85, Animations.DefaultDuration);
        }

        private async Task AnimateMainWindowOut(TimeSpan duration)
        {
            MainWindow.PopupOverlay.IsHitTestVisible = false;
            MainWindow.PopupContainer.IsHitTestVisible = false;
            await MainWindow.PopupOverlay.FadeTo(0, Animations.DefaultDuration);
            MainWindow.PopupContainer.Children.Remove(this);
        }
    }
}
