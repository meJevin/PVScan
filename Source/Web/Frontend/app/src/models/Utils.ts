
import Barcode from "./Barcode";

export function HashOf(barcode: Barcode): string {
    let input = "";

    input += barcode.BarcodeFormat.toString();
    input += barcode.Text.toString() + " ";
    input += barcode.ScanLocation?.Latitude?.toString().replace(",", ".") + " ";
    input += barcode.ScanLocation?.Longitude?.toString().replace(",", ".") + " ";
    input += barcode.ScanTime.getTime().toString() + " ";
    input += barcode.Favorite.toString() + " ";
    input += barcode.GUID?.toString() + " ";

    return "";
}

export function BarcodeFormatToString(barcode: Barcode): string {
    if (barcode.BarcodeFormat == 0) {
        return "AZTEC";
    }
    else if (barcode.BarcodeFormat == 1) {
        return "CODABAR";
    }
    else if (barcode.BarcodeFormat == 2) {
        return "CODE 39";
    }
    else if (barcode.BarcodeFormat == 3) {
        return "CODE 93";
    }
    else if (barcode.BarcodeFormat == 4) {
        return "CODE 128";
    }
    else if (barcode.BarcodeFormat == 5) {
        return "DATA MATRIX";
    }
    else if (barcode.BarcodeFormat == 6) {
        return "EAN 8";
    }
    else if (barcode.BarcodeFormat == 7) {
        return "EAN 13";
    }
    else if (barcode.BarcodeFormat == 8) {
        return "ITF";
    }
    else if (barcode.BarcodeFormat == 9) {
        return "MAXICODE";
    }
    else if (barcode.BarcodeFormat == 10) {
        return "PDF 417";
    }
    else if (barcode.BarcodeFormat == 11) {
        return "QR CODE";
    }
    else if (barcode.BarcodeFormat == 12) {
        return "RSS 14";
    }
    else if (barcode.BarcodeFormat == 13) {
        return "RSS EXPANDED";
    }
    else if (barcode.BarcodeFormat == 14) {
        return "UPC A";
    }
    else if (barcode.BarcodeFormat == 15) {
        return "UPC E";
    }
    else if (barcode.BarcodeFormat == 15) {
        return "UPC EAN EXTENSION";
    }

    return "?";
}
