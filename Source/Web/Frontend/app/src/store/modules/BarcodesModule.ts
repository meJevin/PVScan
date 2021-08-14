import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';
import Barcode from "@/models/Barcode";

@Module({dynamic: true, name: "Barcodes", store: store})
export class BarcodesModule extends VuexModule {

    Barcodes: Barcode[] = [
        {
            Id: 0,
            BarcodeFormat: 24,
            Text: "Something 1",
            ScanLocation: {
                Latitude: 29,
                Longitude: 30,
            },
            ScanTime: new Date(2000, 0, 1),
            Favorite: false,
            Hash: "hash1",
            GUID: "guid1",
            LastUpdateTime: new Date(2000, 5, 5),
        },
        {
            Id: 1,
            BarcodeFormat: 24,
            Text: "Something 2",
            ScanLocation: {
                Latitude: 29,
                Longitude: 30,
            },
            ScanTime: new Date(2000, 0, 1),
            Favorite: true,
            Hash: "hash2",
            GUID: "guid2",
            LastUpdateTime: new Date(2000, 5, 5),
        },
        {
            Id: 2,
            BarcodeFormat: 24,
            Text: "Something 3",
            ScanLocation: {
                Latitude: 29,
                Longitude: 30,
            },
            ScanTime: new Date(2000, 0, 1),
            Favorite: false,
            Hash: "hash3",
            GUID: "guid3",
            LastUpdateTime: new Date(2000, 5, 5),
        },
        {
            Id: 3,
            BarcodeFormat: 24,
            Text: "Something 4",
            ScanLocation: {
                Latitude: 29,
                Longitude: 30,
            },
            ScanTime: new Date(2000, 0, 1),
            Favorite: false,
            Hash: "hash4",
            GUID: "guid4",
            LastUpdateTime: new Date(2000, 5, 5),
        },
    ];

    @Mutation
    SetBarcodeFavorite([barcode, newVal]: [Barcode, boolean]) {
        barcode.Favorite = newVal;
    }

    @Action
    async ToggleBarcodeFavorite(barcode: Barcode) {
        let foundBarcode = this.Barcodes.find(b => b == barcode);

        if (!foundBarcode) return;

        if (foundBarcode.Favorite) {
            this.SetBarcodeFavorite([foundBarcode, false]);
        }
        else {
            this.SetBarcodeFavorite([foundBarcode, true]);
        }
    }
}

const BarcodesModuleExported = getModule(BarcodesModule);

export default BarcodesModuleExported;