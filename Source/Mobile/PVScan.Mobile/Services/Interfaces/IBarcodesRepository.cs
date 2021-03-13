using PVScan.Mobile.Models;
using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Mobile.Services.Interfaces
{
    // CRUD for barcodes
    public interface IBarcodesRepository
    {
        // Create
        Task<Barcode> Save(Barcode barcode);

        // Read
        Task<IEnumerable<Barcode>> GetAll();

        // Delete
        Task Delete(Barcode barcode);
    }
}
