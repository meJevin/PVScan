using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Models
{
    public class User
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public ICollection<Barcode> Barcodes { get; set; }
    }
}
