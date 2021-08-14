import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';
import Barcode from "@/models/Barcode";
import BarcodeFormat from "zxing-typescript/src/core/BarcodeFormat";

import IndexedDbBarcodeRepository from '@/services/IndexedDbBarcodeRepository';

@Module({dynamic: true, name: "Barcodes", store: store})
export class BarcodesModule extends VuexModule {


    private readonly repo = new IndexedDbBarcodeRepository();

    Barcodes: Barcode[] = [];
    SelectedBarcodes: Barcode[] = [];

    @Mutation
    SetBarcodeFavorite([barcode, newVal]: [Barcode, boolean]) {
        barcode.Favorite = newVal;
    }

    @Mutation
    SetBarcodes(barcodes: Barcode[]) {
        this.Barcodes = barcodes;
    }

    @Mutation
    SelectBarcode(barcode: Barcode) {
        this.SelectedBarcodes.push(barcode);
    }

    @Mutation
    DeselectBarcode(barcode: Barcode) {
        this.SelectedBarcodes = this.SelectedBarcodes.filter(b => b != barcode);
    }

    @Mutation
    DeleteBarcodes(barcodes: Barcode[]) {
        this.Barcodes = this.Barcodes.filter(b => barcodes.indexOf(b) == -1);
    }

    @Mutation
    ClearSelectedBarcodes() {
        this.SelectedBarcodes = [];
    }

    @Action
    async DeleteSelectedBarcodes() {
        this.DeleteBarcodes(this.SelectedBarcodes);

        await this.repo.Delete(this.SelectedBarcodes);

        this.ClearSelectedBarcodes();
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

        await this.repo.Update([barcode]);
    }

    @Action
    async Initialize() {
        await this.repo.Initialize();

        this.SetBarcodes(await this.repo.GetAll());
    }
}

const BarcodesModuleExported = getModule(BarcodesModule);

export default BarcodesModuleExported;