import Barcode from "@/models/Barcode";
import IBarcodesRepository from "./interfaces/IBarcodeRepository";
import { DBSchema, IDBPDatabase, openDB } from 'idb';

interface PVScanDBSchema extends DBSchema {
    Barcodes: {
        value: Barcode,
        key: number,
        indexes: { 'GUID_indx': string }
    }
}

export default class IndexedDbBarcodeRepository implements IBarcodesRepository {

    private db: IDBPDatabase<PVScanDBSchema>;

    async Initialize(): Promise<void> {
        this.db = await openDB<PVScanDBSchema>("PVscanDB", 1, {
            upgrade(db: IDBPDatabase<PVScanDBSchema>) {
                const store = db.createObjectStore("Barcodes",
                    {
                        autoIncrement: true,
                        keyPath: 'Id'
                    });

                store.createIndex("GUID_indx", "GUID");
            },
        });
    }

    async Save(barcodes: Barcode[]): Promise<void> {
        const tx = this.db.transaction("Barcodes", "readwrite");

        let dbOperations: Promise<number | void>[] = [];
        for (let i = 0; i < barcodes.length; ++i) {
            dbOperations.push(tx.store.add(barcodes[i]));
        }
        dbOperations.push(tx.done);

        await Promise.all(dbOperations);
    }

    async GetAll(): Promise<Barcode[]> {
        const tx = this.db.transaction("Barcodes", "readonly");
        return await tx.store.getAll();
    }

    async Delete(barcodes: Barcode[]): Promise<void> {
        const tx = this.db.transaction("Barcodes", "readwrite");

        for (let i = 0; i < barcodes.length; ++i) {
            if (barcodes[i].Id) {
                if (await tx.store.get(barcodes[i].Id as number)) {
                    await tx.store.delete(barcodes[i].Id as number);
                }
            }
        }
    }

    async Update(barcodes: Barcode[]): Promise<void> {
        const tx = this.db.transaction("Barcodes", "readwrite");

        for (let i = 0; i < barcodes.length; ++i) {
            console.log("looking for ", barcodes[i].Id);
            if (barcodes[i].Id) {
                if (await tx.store.get(barcodes[i].Id as number)) {
                    await tx.store.put(barcodes[i]);
                }
            }
        }
    }

    async FindByGUID(guids: string[]): Promise<Barcode[]> {
        const tx = this.db.transaction("Barcodes", "readwrite");

        const result = (await tx.store.getAll())
            .filter(b => guids.findIndex(g => g == b.GUID) != -1)

        return result;
    }

}