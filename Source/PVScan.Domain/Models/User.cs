using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Models
{
    public class User
    {
        public string Email;

        public string Username;

        public ICollection<Barcode> ScannedBarcodes;
    }
}
