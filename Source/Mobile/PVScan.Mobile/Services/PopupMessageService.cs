using System;
using System.Threading.Tasks;
using PVScan.Core.Services.Interfaces;
using PVScan.Mobile.Services.Interfaces;
using Xamarin.Forms;

namespace PVScan.Mobile.Services
{
    public class PopupMessageService : IPopupMessageService
    {
        Frame PopupContainer;
        Label PopupMessageLabel;

        double ContainerMinScale = 0.925;

        public void Initialize(Frame container, Label msgLabel)
        {
            PopupContainer = container;
            PopupMessageLabel = msgLabel;

            PopupContainer.Opacity = 0;
            PopupContainer.Scale = ContainerMinScale;
        }

        public async Task ShowMessage(string messageText)
        {
            PopupMessageLabel.Text = messageText;

            _ = PopupContainer.ScaleTo(1, 250, Easing.CubicOut);
            await PopupContainer.FadeTo(1, 250, Easing.CubicOut);

            await Task.Delay(1000);

            _ = PopupContainer.ScaleTo(ContainerMinScale, 250, Easing.CubicOut);
            await PopupContainer.FadeTo(0, 250, Easing.CubicOut);
        }
    }
}
