import Barcode from "@/models/Barcode";

export default interface IBarcodesRepository {
    Initialize(): Promise<void>;

    Save(barcodes: Barcode[]): Promise<void>;

    GetAll(): Promise<Barcode[]>;

    Delete(barcodes: Barcode[]): Promise<void>;

    Update(barcodes: Barcode[]): Promise<void>;
    
    FindByGUID(guids: string[]): Promise<Barcode[]>;
}