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
        Task Save(IEnumerable<Barcode> barcodes);

        // Read
        Task<IEnumerable<Barcode>> GetAll();

        // Delete
        Task Delete(Barcode barcode);
        Task Delete(IEnumerable<Barcode> barcodes);


        // Update
        Task Update(Barcode barcode);
        Task Update(IEnumerable<Barcode> barcodes);

        // Find
        Task<Barcode> FindByGUID(string GUID);
        Task<IEnumerable<Barcode>> FindByGUID(IEnumerable<string> GUIDs);
    }
}
