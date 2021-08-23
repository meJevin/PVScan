import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';
import Barcode from "@/models/Barcode";
import BarcodeFormat from "zxing-typescript/src/core/BarcodeFormat";

import IndexedDbBarcodeRepository from '@/services/IndexedDbBarcodeRepository';

@Module({dynamic: true, name: "Barcodes", store: store})
export class BarcodesModule extends VuexModule {
    private readonly BarcodeRepository = new IndexedDbBarcodeRepository();

    private LastSelectedIndex: number = -1;

    CurrentPage: number = 0;
    BarcodesPerPage: number = 25;

    Barcodes: Barcode[] = [];
    BarcodesPaged: Barcode[] = [];
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
        this.LastSelectedIndex = this.Barcodes.indexOf(barcode);
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
    AddPagedBarcode(barcode: Barcode) {
        this.BarcodesPaged.push(barcode);
    }

    @Mutation
    SetCurrentPage(newVal: number) {
        this.CurrentPage = newVal;
    }

    @Action
    async AddBarcodes(barcodes: Barcode[]) {
        barcodes.forEach(b => {
            this.Barcodes.push(b);
        });

        await this.BarcodeRepository.Save(barcodes);
    }

    @Mutation
    ClearSelectedBarcodes() {
        this.SelectedBarcodes = [];
        this.LastSelectedIndex = -1;
    }

    @Action
    async SelectBarcodesShiftClick(barcode: Barcode) {
        const indexOfCurrSelected = this.Barcodes.indexOf(barcode);

        let fromIndex = -1;
        let toIndex = -1;

        if (this.LastSelectedIndex <= indexOfCurrSelected) {
            fromIndex = this.LastSelectedIndex;
            toIndex = indexOfCurrSelected;
        }
        else {
            fromIndex = indexOfCurrSelected;
            toIndex = this.LastSelectedIndex;
        }

        if (fromIndex >= 0 && fromIndex <= this.Barcodes.length - 1 &&
            toIndex >= 0 && toIndex <= this.Barcodes.length - 1) 
        {
            for (let i = fromIndex; i <= toIndex; ++i) {
                this.SelectBarcode(this.Barcodes[i]);
            }
        }
    }

    @Action
    async DeleteSelectedBarcodes() {
        this.DeleteBarcodes(this.SelectedBarcodes);

        await this.BarcodeRepository.Delete(this.SelectedBarcodes);

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

        await this.BarcodeRepository.Update([barcode]);
    }

    @Action
    async InitializeBarcodes() {
        await this.BarcodeRepository.Initialize();

        this.SetBarcodes(await this.BarcodeRepository.GetAll());
        await this.LoadNextPage();
    }

    @Action
    async LoadNextPage() {
        const fromIndx = this.CurrentPage * this.BarcodesPerPage;
        const toIndx = (this.CurrentPage + 1) * this.BarcodesPerPage;

        console.log(fromIndx, toIndx);

        if (toIndx - fromIndx <= 0) { return; }

        for (let i = fromIndx; i < toIndx && i < this.Barcodes.length; ++i) {
            this.AddPagedBarcode(this.Barcodes[i]);
        }

        this.SetCurrentPage(this.CurrentPage + 1);
    }
}

const BarcodesModuleExported = getModule(BarcodesModule);

export default BarcodesModuleExported;