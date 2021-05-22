using System;
using System.IO;
using System.Windows.Input;
using MvvmHelpers;
using PVScan.Mobile.Converters;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Svg;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Core;

namespace PVScan.Mobile.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        readonly IPopupMessageService PopupMessageService;
        readonly IMediaService MediaService;
        readonly IPersistentKVP KVP;

        public BarcodeInfoPageViewModel(
            IPopupMessageService popupMessageService,
            IMediaService mediaService,
            IPersistentKVP kvp)
        {
            PopupMessageService = popupMessageService;
            MediaService = mediaService;
            KVP = kvp;

            TextLongPressCommand = new Command(async () =>
            {
                if (SelectedBarcode == null)
                {
                    return;
                }

                await Clipboard.SetTextAsync(SelectedBarcode.Text);

                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                await PopupMessageService.ShowMessage("Text copied");
            });

            LocationLongPressCommand = new Command(async () =>
            {
                if (SelectedBarcode == null)
                {
                    return;
                }

                var coordToStr = new CoordinateStringConverter();
                var coordStr = (string)(coordToStr.Convert(
                    SelectedBarcode.ScanLocation, null, null, null));
                await Clipboard.SetTextAsync(coordStr);

                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                await PopupMessageService.ShowMessage("Location copied");
            });

            FormatLongPressCommand = new Command(async () =>
            {
                if (SelectedBarcode == null)
                {
                    return;
                }

                var formatToStr = new BarcodeFormatStringConverter();
                var formatStr = (string)(formatToStr.Convert(
                    SelectedBarcode.Format, null, null, null));
                await Clipboard.SetTextAsync(formatStr);

                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                await PopupMessageService.ShowMessage("Format copied");
            });

            ScanTimeLongPressCommand = new Command(async () =>
            {
                if (SelectedBarcode == null)
                {
                    return;
                }

                await Clipboard.SetTextAsync(SelectedBarcode.ScanTime.ToString());

                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                await PopupMessageService.ShowMessage("Scan time copied");
            });

            BarcodeImageLongPressCommand = new Command(async () =>
            {
                if (SelectedBarcode == null)
                {
                    return;
                }

                bool keepAlpha = KVP.Get(StorageKeys.SaveBarcodeImagesWithAlpha, true);

                var barcodeToImage = new BarcodeImageConverter();
                var barcodeImage = (barcodeToImage.Convert(
                    SelectedBarcode, null, null, null)) as SvgImageSource;
                var barcodeImagePath = Path.Combine(FileSystem.CacheDirectory, "PVScan_Temp");

                // PNG supports alpha, otherwise make it a JPEG
                if (keepAlpha)
                {
                    barcodeImagePath += ".png";
                }
                else
                {
                    barcodeImagePath += ".jpeg";
                }

                HapticFeedback.Perform(HapticFeedbackType.LongPress);

                await MediaService.SaveSvgImage(barcodeImage, barcodeImagePath);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Saving barcode image from PVScan",
                    File = new ShareFile(barcodeImagePath),
                });

                //await PopupMessageService.ShowMessage("Saved barcode image to gallery");
            });
        }

        public Barcode SelectedBarcode { get; set; }

        //public ICommand DeleteCommand { get; set; }
        //public ICommand ShowOnMapCommand { get; set; }
        //public ICommand ShowOnListCommand { get; set; }

        public ICommand TextLongPressCommand { get; set; }
        public ICommand LocationLongPressCommand { get; set; }
        public ICommand FormatLongPressCommand { get; set; }
        public ICommand ScanTimeLongPressCommand { get; set; }
        public ICommand BarcodeImageLongPressCommand { get; set; }
    }
}
