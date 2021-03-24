using System;
using System.Threading.Tasks;
using PVScan.Mobile.Services.Interfaces;
using Xamarin.Forms;

namespace PVScan.Mobile.Services
{
    public class PopupMessageService : IPopupMessageService
    {
        Grid PopupContainer;
        Label PopupMessageLabel;

        public void Initialize(Grid container, Label msgLabel)
        {
            PopupContainer = container;
            PopupMessageLabel = msgLabel;
        }

        public async Task ShowMessage(string messageText)
        {
            PopupMessageLabel.Text = messageText;

            await PopupContainer.FadeTo(1, 250, Easing.CubicOut);
            await Task.Delay(1000);
            await PopupContainer.FadeTo(0, 250, Easing.CubicOut);
        }
    }
}
