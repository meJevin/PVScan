using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PVScan.Mobile.Services
{
    public class XamarinAPIBarcodeHubFacade : IAPIBarcodeHub
    {
        public event EventHandler<ScannedBarcodeRequest> OnScanned;
        public event EventHandler<DeletedBarcodeRequest> OnDeleted;
        public event EventHandler<UpdatedBarcodeRequest> OnUpdated;
        public event EventHandler<List<ScannedBarcodeRequest>> OnScannedMultiple;
        public event EventHandler<List<DeletedBarcodeRequest>> OnDeletedMultiple;
        public event EventHandler<List<UpdatedBarcodeRequest>> OnUpdatedMultple;

        IAPIBarcodeHub _normalHub;

        public XamarinAPIBarcodeHubFacade(IAPIBarcodeHub barcodeHub)
        {
            _normalHub = barcodeHub;

            _normalHub.OnScanned += Hub_OnScanned;
            _normalHub.OnDeleted += Hub_OnDeleted;
            _normalHub.OnUpdated += Hub_OnUpdated;

            _normalHub.OnScannedMultiple += Hub_OnScannedMultiple;
            _normalHub.OnDeletedMultiple += Hub_OnDeletedMultiple;
            _normalHub.OnUpdatedMultple += Hub_OnUpdatedMultple;
        }

        private void Hub_OnUpdatedMultple(object sender, List<UpdatedBarcodeRequest> e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnUpdatedMultple?.Invoke(sender, e);
            });
        }

        private void Hub_OnDeletedMultiple(object sender, List<DeletedBarcodeRequest> e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnDeletedMultiple?.Invoke(sender, e);
            });
        }

        private void Hub_OnScannedMultiple(object sender, List<ScannedBarcodeRequest> e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnScannedMultiple?.Invoke(sender, e);
            });
        }

        private void Hub_OnUpdated(object sender, UpdatedBarcodeRequest e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnUpdated?.Invoke(sender, e);
            });
        }

        private void Hub_OnDeleted(object sender, DeletedBarcodeRequest e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnDeleted?.Invoke(sender, e);
            });
        }

        private void Hub_OnScanned(object sender, ScannedBarcodeRequest e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                OnScanned?.Invoke(sender, e);
            });
        }

        public Task Connect()
        {
            return _normalHub.Connect();
        }

        public Task Deleted(DeletedBarcodeRequest b)
        {
            return _normalHub.Deleted(b);
        }

        public Task DeletedMultiple(List<DeletedBarcodeRequest> reqs)
        {
            return _normalHub.DeletedMultiple(reqs);
        }

        public Task Disconnect()
        {
            return _normalHub.Disconnect();
        }

        public Task Scanned(ScannedBarcodeRequest b)
        {
            return _normalHub.Scanned(b);
        }

        public Task ScannedMultiple(List<ScannedBarcodeRequest> reqs)
        {
            return _normalHub.ScannedMultiple(reqs);
        }

        public Task Updated(UpdatedBarcodeRequest b)
        {
            return _normalHub.Updated(b);
        }

        public Task UpdatedMultiple(List<UpdatedBarcodeRequest> reqs)
        {
            return _normalHub.UpdatedMultiple(reqs);
        }
    }
}
