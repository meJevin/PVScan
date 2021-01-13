using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Enums
{
    // This is literally stolen from ZXing
    public enum BarcodeFormat
    {
        //
        // Summary:
        //     Aztec 2D barcode format.
        AZTEC = 1,
        //
        // Summary:
        //     CODABAR 1D format.
        CODABAR = 2,
        //
        // Summary:
        //     Code 39 1D format.
        CODE_39 = 4,
        //
        // Summary:
        //     Code 93 1D format.
        CODE_93 = 8,
        //
        // Summary:
        //     Code 128 1D format.
        CODE_128 = 16,
        //
        // Summary:
        //     Data Matrix 2D barcode format.
        DATA_MATRIX = 32,
        //
        // Summary:
        //     EAN-8 1D format.
        EAN_8 = 64,
        //
        // Summary:
        //     EAN-13 1D format.
        EAN_13 = 128,
        //
        // Summary:
        //     ITF (Interleaved Two of Five) 1D format.
        ITF = 256,
        //
        // Summary:
        //     MaxiCode 2D barcode format.
        MAXICODE = 512,
        //
        // Summary:
        //     PDF417 format.
        PDF_417 = 1024,
        //
        // Summary:
        //     QR Code 2D barcode format.
        QR_CODE = 2048,
        //
        // Summary:
        //     RSS 14
        RSS_14 = 4096,
        //
        // Summary:
        //     RSS EXPANDED
        RSS_EXPANDED = 8192,
        //
        // Summary:
        //     UPC-A 1D format.
        UPC_A = 16384,
        //
        // Summary:
        //     UPC-E 1D format.
        UPC_E = 32768,
        //
        // Summary:
        //     UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 | ITF
        //     | RSS_14 | RSS_EXPANDED without MSI (to many false-positives) and IMB (not enough
        //     tested, and it looks more like a 2D)
        All_1D = 61918,
        //
        // Summary:
        //     UPC/EAN extension format. Not a stand-alone format.
        UPC_EAN_EXTENSION = 65536,
        //
        // Summary:
        //     MSI
        MSI = 131072,
        //
        // Summary:
        //     Plessey
        PLESSEY = 262144,
        //
        // Summary:
        //     Intelligent Mail barcode
        IMB = 524288,
        //
        // Summary:
        //     Pharmacode format.
        PHARMA_CODE = 1048576
    }
}
