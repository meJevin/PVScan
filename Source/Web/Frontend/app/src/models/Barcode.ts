import Coordinate from "./Coordinate";

export default interface Barcode {
    Id?: number | undefined;
    BarcodeFormat: number; // Later convert to something more mnemonic
    Text: string;
    ScanLocation: Coordinate;
    ScanTime: Date;
    Favorite: boolean;
    Hash: string;
    GUID: string;
    LastUpdateTime: Date;
}
