import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';
import Barcode from "@/models/Barcode";
import BarcodeFormat from "zxing-typescript/src/core/BarcodeFormat";

import {Filter, EmptyFilter, IsEmptyFilter,LastTimeType} from "@/models/Filter";
import {DefaultSorting,IsDefaultSorting,Sorting,SortingField} from "@/models/Sorting";

import SortingService from "@/services/SortingService";

import IndexedDbBarcodeRepository from '@/services/IndexedDbBarcodeRepository';
import FilterService from "@/services/FilterService";

@Module({dynamic: true, name: "barcodes", store: store})
class BarcodesModule extends VuexModule {
    private readonly BarcodeRepository = new IndexedDbBarcodeRepository();
    private readonly SortingService = new SortingService();
    private readonly FilterService = new FilterService();

    private lastSelectedIndex: number = -1;

    private currentPage: number = 0;
    private barcodesPerPage: number = 25;

    private barcodes: Barcode[] = [];
    private barcodesPaged: Barcode[] = [];
    private selectedBarcodes: Barcode[] = [];

    private currentFilter: Filter = EmptyFilter();
    private currentSorting: Sorting = DefaultSorting();

    get CurrentPage(): number {
        return this.currentPage;
    }
    protected set CurrentPage(newVal: number) {
        this.currentPage = newVal;
    }

    get BarcodesPerPage(): number {
        return this.barcodesPerPage;
    }

    get Barcodes(): Barcode[] {
        return this.barcodes;
    }
    get BarcodesPaged(): Barcode[] {
        return this.barcodesPaged;
    }
    get SelectedBarcodes(): Barcode[] {
        return this.selectedBarcodes;
    }

    get CurrentSorting(): Sorting {
        return this.currentSorting;
    }
    protected set CurrentSorting(newValue: Sorting) {
        this.currentSorting = newValue;
    }

    get CurrentFilter(): Filter {
        return this.currentFilter;
    }
    protected set CurrentFilter(newValue: Filter) {
        this.currentFilter = newValue;
    }

    @Mutation
    SetBarcodeFavorite([barcode, newVal]: [Barcode, boolean]) {
        barcode.Favorite = newVal;
    }

    @Mutation
    SetSorting(newSorting: Sorting) {
        this.currentSorting = newSorting;
    }

    @Mutation
    SetFilter(newFilter: Filter) {
        this.currentFilter = newFilter;
    }

    @Mutation
    SetBarcodes(barcodes: Barcode[]) {
        this.barcodes = barcodes;
    }

    @Mutation
    SelectBarcode(barcode: Barcode) {
        this.selectedBarcodes.push(barcode);
        this.lastSelectedIndex = this.barcodes.indexOf(barcode);
    }

    @Mutation
    DeselectBarcode(barcode: Barcode) {
        this.selectedBarcodes = this.selectedBarcodes.filter(b => b != barcode);
    }

    @Mutation
    DeleteBarcodes(barcodes: Barcode[]) {
        this.barcodes = this.barcodes.filter(b => barcodes.indexOf(b) == -1);
    }

    @Mutation
    AddPagedBarcode(barcode: Barcode) {
        this.barcodesPaged.push(barcode);
    }

    @Mutation
    SetCurrentPage(newVal: number) {
        this.currentPage = newVal;
    }

    @Action
    async AddBarcodes(barcodes: Barcode[]) {
        barcodes.forEach(b => {
            this.barcodes.push(b);
        });

        await this.BarcodeRepository.Save(barcodes);
    }

    @Mutation
    ClearSelectedBarcodes() {
        this.selectedBarcodes = [];
        this.lastSelectedIndex = -1;
    }

    @Mutation
    ClearLoadedBarcodes() {
        this.barcodes = [];
        this.barcodesPaged = [];
        this.currentPage = 0;
    }

    @Action
    async SelectBarcodesShiftClick(barcode: Barcode) {
        const indexOfCurrSelected = this.barcodes.indexOf(barcode);

        let fromIndex = -1;
        let toIndex = -1;

        if (this.lastSelectedIndex <= indexOfCurrSelected) {
            fromIndex = this.lastSelectedIndex;
            toIndex = indexOfCurrSelected;
        }
        else {
            fromIndex = indexOfCurrSelected;
            toIndex = this.lastSelectedIndex;
        }

        if (fromIndex >= 0 && fromIndex <= this.barcodes.length - 1 &&
            toIndex >= 0 && toIndex <= this.barcodes.length - 1) 
        {
            for (let i = fromIndex; i <= toIndex; ++i) {
                this.SelectBarcode(this.barcodes[i]);
            }
        }
    }

    @Action
    async DeleteSelectedBarcodes() {
        this.DeleteBarcodes(this.selectedBarcodes);

        await this.BarcodeRepository.Delete(this.selectedBarcodes);

        this.ClearSelectedBarcodes();
    }

    @Action
    async ToggleBarcodeFavorite(barcode: Barcode) {
        let foundBarcode = this.barcodes.find(b => b == barcode);

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

        await this.LoadBarcodesFromDB();
        await this.LoadNextPage();
    }

    @Action 
    async LoadBarcodesFromDB() {
        this.ClearLoadedBarcodes();
        this.ClearSelectedBarcodes();

        let barcodesFromDB = await this.BarcodeRepository.GetAll();

        barcodesFromDB = await this.SortingService.Sort(barcodesFromDB, this.currentSorting);

        if (!IsEmptyFilter(this.currentFilter)) {
            barcodesFromDB = await this.FilterService.Filter(barcodesFromDB, this.currentFilter);
        }

        this.SetBarcodes(barcodesFromDB);
    }

    @Action
    async LoadNextPage() {
        const fromIndx = this.currentPage * this.barcodesPerPage;
        const toIndx = (this.currentPage + 1) * this.barcodesPerPage;

        console.log(fromIndx, toIndx);

        if (toIndx - fromIndx <= 0) { return; }

        for (let i = fromIndx; i < toIndx && i < this.barcodes.length; ++i) {
            this.AddPagedBarcode(this.barcodes[i]);
        }

        this.SetCurrentPage(this.currentPage + 1);
    }

    @Action
    async ApplyFilter(newFilter: Filter) {
        this.SetFilter(newFilter);

        await this.LoadBarcodesFromDB();
        await this.LoadNextPage();
    }

    @Action
    async ApplySorting(newSorting: Sorting) {
        this.SetSorting(newSorting);

        await this.LoadBarcodesFromDB();
        await this.LoadNextPage();
    }
}

const BarcodesModuleExported = getModule(BarcodesModule);

export default BarcodesModuleExported;