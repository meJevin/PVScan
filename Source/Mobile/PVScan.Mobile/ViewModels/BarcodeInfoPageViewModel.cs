﻿using System;
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
using PVScan.Core.Models.API;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Mobile.ViewModels
{
    public class BarcodeInfoPageViewModel : BaseViewModel
    {
        readonly IPopupMessageService PopupMessageService;
        readonly IMediaService MediaService;
        readonly IPersistentKVP KVP;
        readonly IAPIBarcodeHub BarcodeHub;
        readonly IBarcodesRepository BarcodesRepository;

        public BarcodeInfoPageViewModel(
            IPopupMessageService popupMessageService,
            IMediaService mediaService,
            IPersistentKVP kvp, 
            IAPIBarcodeHub barcodeHub,
            IBarcodesRepository barcodesRepository)
        {
            PopupMessageService = popupMessageService;
            MediaService = mediaService;
            KVP = kvp;
            BarcodeHub = barcodeHub;
            BarcodesRepository = barcodesRepository;

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

                bool keepAlpha = await KVP.Get(StorageKeys.SaveBarcodeImagesWithAlpha, true);

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

            BarcodeHub.OnUpdated += BarcodeHub_OnUpdated;
            BarcodeHub.OnUpdatedMultple += BarcodeHub_OnUpdatedMultple;

            BarcodeHub.OnDeleted += BarcodeHub_OnDeleted;
            BarcodeHub.OnDeletedMultiple += BarcodeHub_OnDeletedMultiple;

            CloseCommand = new Command(() =>
            {
                Closed?.Invoke(this, new EventArgs());
            });
        }
        
        private async void BarcodeHub_OnUpdated(object sender, UpdatedBarcodeRequest req)
        {
            if (SelectedBarcode == null ||
                req.GUID != SelectedBarcode.GUID)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(req.GUID);

            if (localBarcode == null)
            {
                return;
            }

            if (req.Longitude.HasValue && req.Latitude.HasValue)
            {
                localBarcode.ScanLocation = new Coordinate()
                {
                    Latitude = req.Latitude,
                    Longitude = req.Longitude,
                };
            }
            localBarcode.Favorite = req.Favorite;

            SelectedBarcode = null;
            SelectedBarcode = localBarcode;
        }

        private async void BarcodeHub_OnUpdatedMultple(object sender, List<UpdatedBarcodeRequest> e)
        {
            if (SelectedBarcode == null) return;

            var found = e.FirstOrDefault(d => d.GUID == SelectedBarcode.GUID);

            if (found == null)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(found.GUID);

            if (localBarcode == null)
            {
                return;
            }

            if (found.Longitude.HasValue && found.Latitude.HasValue)
            {
                localBarcode.ScanLocation = new Coordinate()
                {
                    Latitude = found.Latitude,
                    Longitude = found.Longitude,
                };
            }
            localBarcode.Favorite = found.Favorite;

            SelectedBarcode = null;
            SelectedBarcode = localBarcode;
        }

        private async void BarcodeHub_OnDeletedMultiple(object sender, List<DeletedBarcodeRequest> e)
        {
            if (SelectedBarcode == null) return;

            var found = e.FirstOrDefault(d => d.GUID == SelectedBarcode.GUID);

            if (SelectedBarcode == null ||
                found == null)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(found.GUID);

            if (localBarcode == null)
            {
                return;
            }

            SelectedBarcode = null;

            Closed?.Invoke(this, new EventArgs());
        }

        private async void BarcodeHub_OnDeleted(object sender, DeletedBarcodeRequest e)
        {
            if (SelectedBarcode == null ||
                e.GUID != SelectedBarcode.GUID)
            {
                return;
            }

            var localBarcode = await BarcodesRepository.FindByGUID(e.GUID);

            if (localBarcode == null)
            {
                return;
            }

            SelectedBarcode = null;

            Closed?.Invoke(this, new EventArgs());
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

        public ICommand CloseCommand { get; set; }
        public event EventHandler Closed;
    }
}
