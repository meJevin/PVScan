
import Barcode, { BarcodeFormat } from "./Barcode";
import { LastTimeType } from "./Filter";

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

export function BarcodeFormatToString(format: BarcodeFormat): string {
    const str = BarcodeFormat[format];

    return str.charAt(0).toUpperCase() + str.replace('_', ' ').toLowerCase().slice(1);
}

export function LastTimeTypeToString(ltt: LastTimeType): string {
    return LastTimeType[ltt];
}
