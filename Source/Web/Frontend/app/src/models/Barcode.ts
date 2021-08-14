import Coordinate from "./Coordinate";
import BarcodeFormat from "zxing-typescript/src/core/BarcodeFormat";

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

export default interface Barcode {
    Id: number;
    BarcodeFormat: number; // Later convert to something more mnemonic
    Text: string;
    ScanLocation: Coordinate;
    ScanTime: Date;
    Favorite: boolean;
    Hash: string;
    GUID: string;
    LastUpdateTime: Date;
}
