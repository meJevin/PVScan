using PVScan.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services.Interfaces
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

        // Update
        Task Update(Barcode barcode);

        Task<Barcode> FindByGUID(string GUID);
    }
}
