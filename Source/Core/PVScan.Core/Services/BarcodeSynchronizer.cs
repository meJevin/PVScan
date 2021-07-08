using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services
{
    public class BarcodeSynchronizer : IBarcodeSynchronizer
    {
        readonly IBarcodesRepository BarcodesRepository;
        readonly IPVScanAPI API;
        readonly IAPIBarcodeHub BarcodeHub;

        public BarcodeSynchronizer(
            IBarcodesRepository barcodesRepository,
            IPVScanAPI api,
            IAPIBarcodeHub barcodeHub)
        {
            BarcodesRepository = barcodesRepository;
            API = api;
            BarcodeHub = barcodeHub;
        }

        public event EventHandler SynchorinizedLocally;

        public async Task Synchronize()
        {
            var barcodesGUIDsAndHashes = (await BarcodesRepository.GetAll())
                .Select(b => new LocalBarcodeInfo
                {
                    GUID = b.GUID,
                    LocalId = b.Id,
                    Hash = b.Hash,
                    LastTimeUpdated = b.LastUpdateTime,
                })
                .OrderBy(i => i.LocalId);

            var result = await API.Synchronize(new SynchronizeRequest()
            {
                LocalBarcodeInfos = barcodesGUIDsAndHashes
            });

            if (result == null)
            {
                return;
            }

            foreach (var add in result.ToAddLocaly)
            {
                await BarcodesRepository.Save(add);
            }

            foreach (var update in result.ToUpdateLocaly)
            {
                var dbBarcode = await BarcodesRepository.FindByGUID(update.GUID);

                dbBarcode.Favorite = update.Favorite;
                dbBarcode.Format = update.Format;
                dbBarcode.ScanLocation = update.ScanLocation;
                dbBarcode.ScanTime = update.ScanTime;
                dbBarcode.Text = update.Text;
                await BarcodesRepository.Update(dbBarcode);
            }

            foreach (var addToserver in result.ToAddToServer)
            {
                // Send to server as scanned API
                var b = await BarcodesRepository.FindByGUID(addToserver);

                var req = new ScannedBarcodeRequest()
                {
                    Favorite = b.Favorite,
                    Format = b.Format,
                    GUID = b.GUID,
                    Hash = b.Hash,
                    ScanTime = b.ScanTime,
                    Text = b.Text,
                    LastTimeUpdated = b.LastUpdateTime,
                };

                if (b.ScanLocation != null)
                {
                    req.Latitude = b.ScanLocation.Latitude.Value;
                    req.Longitude = b.ScanLocation.Longitude.Value;
                }

                await API.ScannedBarcode(req);
                await BarcodeHub.Scanned(req);
            }


            foreach (var updateOnServer in result.ToUpdateOnServer)
            {
                // Send to server as scanned API
                var b = await BarcodesRepository.FindByGUID(updateOnServer);

                var req = new UpdatedBarcodeRequest()
                {
                    Favorite = b.Favorite,
                    GUID = b.GUID,
                    LastTimeUpdated = b.LastUpdateTime,
                };

                if (b.ScanLocation != null)
                {
                    req.Latitude = b.ScanLocation.Latitude.Value;
                    req.Longitude = b.ScanLocation.Longitude.Value;
                }

                await API.UpdatedBarcode(req);
                await BarcodeHub.Updated(req);
            }

            if (result.ToAddLocaly.Count() != 0 || result.ToUpdateLocaly.Count() != 0)
            {
                SynchorinizedLocally?.Invoke(this, new EventArgs());
            }
        }
    }
}
