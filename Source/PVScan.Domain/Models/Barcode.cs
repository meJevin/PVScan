using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Models
{
    public class Barcode
    {
        public int Id { get; set; }

        /// <summary>
        /// Format of scanned barcode
        /// </summary>
        public BarcodeFormat Format { get; set; }

        /// <summary>
        /// Raw bytes of scanned barcode
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// Link to source image of barcode
        /// </summary>
        public string ImageURL { get; set; }

        /// <summary>
        /// When the barcode has been scanned
        /// </summary>
        public DateTime ScannedAt { get; set; }

        /// <summary>
        /// Where the barcode has been scanned
        /// </summary>
        public Coordinate Location { get; set; }

        public int UserId { get; set; }
    }
}
