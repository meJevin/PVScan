using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Scanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace PVScan.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ApplicationSettingsPage : ContentView
    {
        public event EventHandler BackClicked;
        ApplicationSettingsPageViewModel VM;

        readonly IBarcodesRepository BarcodesRepository;

        public ApplicationSettingsPage()
        {
            BarcodesRepository = Resolver.Resolve<IBarcodesRepository>();

            InitializeComponent();

            VM = BindingContext as ApplicationSettingsPageViewModel;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            BackClicked.Invoke(sender, e);
        }

        private void DarkTheme_Toggled(object sender, ToggledEventArgs e)
        {
            if (VM == null) return;

            VM.SwitchThemeCommand.Execute(null);
        }

        private void KeepAlpha_Toggled(object sender, ToggledEventArgs e)
        {
            if (VM == null) return;

            VM.SwitchSaveBarcodeImagesWithAlphaCommand.Execute(null);
        }

        #region DEBUG STUFF
        private void GenerateBarcode()
        {
            Array values = Enum.GetValues(typeof(BarcodeFormat)).OfType<BarcodeFormat>().Where(f => { return (int)f <= 2048; }).ToArray();
            Random random = new Random();
            BarcodeFormat randomType = (BarcodeFormat)values.GetValue(random.Next(values.Length));

            DateTime date = DateTime.UtcNow;
            date = date.Subtract(TimeSpan.FromSeconds(random.NextDouble() * 157680000));

            Coordinate randomCoord = new Coordinate()
            {
                Latitude = (random.NextDouble() * 180) - 90,
                Longitude = (random.NextDouble() * 360) - 180,
            };

            var b = new Barcode()
            {
                Format = randomType,
                ScanLocation = random.NextDouble() > 0.25 ? randomCoord : null,
                ScanTime = date,
                Text = Guid.NewGuid().ToString(),
            };

            BarcodesRepository.Save(b);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            GenerateBarcode();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                GenerateBarcode();
            }
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            for (int i = 0; i < 100; ++i)
            {
                GenerateBarcode();
            }
        }
        #endregion
    }
}