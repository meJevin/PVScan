using PVScan.Core.DAL;
using PVScan.Core.Models;
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

        private bool _isSyncing = false;

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
            if (_isSyncing) return;

            _isSyncing = true;

            var localBarcodes = (await BarcodesRepository.GetAll())
                .ToDictionary(b => b.GUID);

            var badLocalHashes = new List<Barcode>();
            foreach (var b in localBarcodes)
            {
                var hash = Barcode.HashOf(b.Value);

                if (b.Value.Hash != hash)
                {
                    badLocalHashes.Add(b.Value);
                }
            }

            await BarcodesRepository.Update(badLocalHashes);

            var localBarcodeInfos = localBarcodes
                .OrderBy(i => i.Value.Id)
                .ToList()
                .Select(b => new LocalBarcodeInfo
                {
                    GUID = b.Value.GUID,
                    LocalId = b.Value.Id,
                    Hash = b.Value.Hash,
                    LastTimeUpdated = b.Value.LastUpdateTime,
                });

            var result = await API.Synchronize(new SynchronizeRequest()
            {
                LocalBarcodeInfos = localBarcodeInfos
            });

            if (result == null)
            {
                _isSyncing = false;
                return;
            }

            await BarcodesRepository.Save(result.ToAddLocaly);

            var localUpd = new List<Barcode>();
            foreach (var b in result.ToUpdateLocaly)
            {
                if(localBarcodes.TryGetValue(b.GUID, out var found))
                {
                    localUpd.Add(b);
                }
            }

            await BarcodesRepository.Update(localUpd);

            var scanReqs = new List<ScannedBarcodeRequest>();
            foreach (var guid in result.ToAddToServer)
            {
                if (!localBarcodes.TryGetValue(guid, out var addToServer))
                {
                    continue;
                }

                // Send to server as scanned API
                var req = new ScannedBarcodeRequest()
                {
                    Favorite = addToServer.Favorite,
                    Format = addToServer.Format,
                    GUID = addToServer.GUID,
                    Hash = addToServer.Hash,
                    ScanTime = addToServer.ScanTime,
                    Text = addToServer.Text,
                    LastTimeUpdated = addToServer.LastUpdateTime,
                };

                if (addToServer.ScanLocation != null)
                {
                    req.Latitude = addToServer.ScanLocation.Latitude.Value;
                    req.Longitude = addToServer.ScanLocation.Longitude.Value;
                }

                scanReqs.Add(req);
            }

            var updReqs = new List<UpdatedBarcodeRequest>();
            foreach (var guid in result.ToUpdateOnServer)
            {
                if (!localBarcodes.TryGetValue(guid, out var updateOnServer))
                {
                    continue;
                }

                // Send to server as scanned API
                var req = new UpdatedBarcodeRequest()
                {
                    Favorite = updateOnServer.Favorite,
                    GUID = updateOnServer.GUID,
                    LastTimeUpdated = updateOnServer.LastUpdateTime,
                };

                if (updateOnServer.ScanLocation != null)
                {
                    req.Latitude = updateOnServer.ScanLocation.Latitude.Value;
                    req.Longitude = updateOnServer.ScanLocation.Longitude.Value;
                }

                updReqs.Add(req);
            }

            if (updReqs.Count != 0)
            {
                await API.UpdatedBarcodeMultiple(updReqs);
                await BarcodeHub.UpdatedMultiple(updReqs);
            }

            if (scanReqs.Count != 0)
            {
                await API.ScannedBarcodeMultiple(scanReqs);
                await BarcodeHub.ScannedMultiple(scanReqs);
            }

            _isSyncing = false;

            if (result.ToAddLocaly.Count() > 0 || result.ToUpdateLocaly.Count() > 0)
            {
                SynchorinizedLocally?.Invoke(this, new EventArgs());
            }
        }
    }
}
