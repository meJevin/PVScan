<template>
    <div class="filter">
        <font-awesome-icon class="close-icon" 
        icon="times" color="white" @click="onCloseButtonClicked"/>

        <h1 class="flex-shrink-0">
            Filter Barcodes
        </h1>

        <div class="separator flex-shrink-0"></div>

        <div class="content">
            <div class="filter-header">
                <p>
                    Date 
                </p>

                <p class="reset-button" @click="onResetDateButtonClicked">Reset</p>
            </div>

            <div class="available-last-time-types">
                <div v-for="ltt of availableLastTimeTypes"
                    :key="ltt" @click="handleLastTimeTypeClick(ltt)"
                    :class="['filter-field', lastTimeTypesEqual(selectedLastTimeType, ltt) ? 'selected' : '']">
                    {{getLastTimeTypeString(ltt)}}

                    <font-awesome-icon v-if="lastTimeTypesEqual(selectedLastTimeType, ltt)" 
                        class="check-icon" icon="check" color="white"/>
                </div>
            </div>

            <div class="filter-header">
                <p>
                    Barcode Formats 
                </p>

                <p class="reset-button" @click="onResetBarcodeFormatsButtonClicked">Reset</p>
            </div>

            <div class="available-barcode-formats">
                <div v-for="bFormat of availableBarcodeFormats"
                    :key="bFormat" @click="handleBarcodeFormatClick(bFormat)"
                    :class="['filter-field', barcodeFormatContains(selectedBarcodeFormats, bFormat) ? 'selected' : '']">
                    {{getBarcodeFormatString(bFormat)}}

                    <font-awesome-icon v-if="barcodeFormatContains(selectedBarcodeFormats, bFormat)" 
                        class="check-icon" icon="check" color="white"/>
                </div>
            </div>
        </div>

        <button :class="['primary-button medium flex-shrink-0',
                        CurrentFilterSameAsState ? 'disabled' : '']" 
                @click="onApplyButtonClicked"
                style="margin-top: 15px;">
            Apply
        </button>
    </div>
</template>

<script lang="ts">
import { Component, Emit, Prop, Vue } from "vue-property-decorator";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import BarcodesModule from "../store/modules/BarcodesModule";
import { EmptyFilter, Filter, LastTimeType } from "../models/Filter";
import { BarcodeFormat } from "../models/Barcode";
import { BarcodeFormatToString, LastTimeTypeToString } from "../models/Utils";

@Component({
    components: {
        FontAwesomeIcon,
    },
})
export default class FilterComponent extends Vue {
    manualDateRangeSelected: boolean = false; // from to...

    fromDate?: Date = new Date();
    toDate?: Date = new Date();

    availableLastTimeTypes: LastTimeType[] = [];
    selectedLastTimeType: LastTimeType = LastTimeType.Day;

    availableBarcodeFormats: BarcodeFormat[] = [];
    selectedBarcodeFormats: BarcodeFormat[] = [];

    get CurrentFilterSameAsState(): boolean {
        const currentFilter = JSON.stringify(BarcodesModule.CurrentFilter);
        const stateFilter = JSON.stringify(this.StateFilter);

        return currentFilter === stateFilter;
    }

    get StateFilter(): Filter {
        const selectedBrcdFrmts 
            = this.selectedBarcodeFormats.length == 0 ? 
            undefined : this.selectedBarcodeFormats.map(el => el);
        
        const stateFilter: Filter = {
            FromDate: this.fromDate ? new Date(this.fromDate) : undefined,
            ToDate: this.toDate ? new Date(this.toDate) : undefined,
            LastType: this.selectedLastTimeType,
            BarcodeFormats: selectedBrcdFrmts
        };

        return stateFilter;
    }

    handleLastTimeTypeClick(val: LastTimeType) {
        if (this.lastTimeTypesEqual(this.selectedLastTimeType, val)) {
            this.selectedLastTimeType = undefined;
            return;
        }

        this.selectedLastTimeType = val;
    }

    handleBarcodeFormatClick(val: BarcodeFormat) {
        const foundIndex = this.selectedBarcodeFormats.indexOf(val);

        if (foundIndex != -1) {
            // Already in, remove
            this.selectedBarcodeFormats.splice(foundIndex, 1);
            return;
        }

        // Not in, add
        this.selectedBarcodeFormats.push(val);
        this.selectedBarcodeFormats.sort();
    }

    onResetDateButtonClicked() {
        const emptyFilter = EmptyFilter();

        this.fromDate = emptyFilter.FromDate;
        this.toDate = emptyFilter.ToDate;
        this.selectedLastTimeType = emptyFilter.LastType;
    }

    onResetBarcodeFormatsButtonClicked() {
        const emptyFilter = EmptyFilter();

        if (emptyFilter.BarcodeFormats) {
            this.selectedBarcodeFormats = emptyFilter.BarcodeFormats;
        }

        this.selectedBarcodeFormats = [];
    }

    //#region EVENTS
    @Emit("closed")
    onCloseButtonClicked() {

    }

    @Emit("filter-applied")
    onApplyButtonClicked() {
        return this.StateFilter;
    }
    //#endregion

    //#region HOOKS
    created() {
        this.initialize();
        this.initializeFromModule();
    }

    initialize() {
        const availableLTTs = Object.values(LastTimeType);
        const availableBrcdFrmts = Object.values(BarcodeFormat);

        availableLTTs.forEach(el => {
            if (typeof(el) === "string") return;
            this.availableLastTimeTypes.push(el);
        });
        availableBrcdFrmts.forEach(el => {
            if (typeof(el) === "string") return;
            this.availableBarcodeFormats.push(el);
        });

        this.manualDateRangeSelected = false;
        this.fromDate = undefined;
        this.toDate = undefined;
        this.selectedLastTimeType = undefined;
        this.selectedBarcodeFormats = [];
    }

    public initializeFromModule() {
        if (BarcodesModule.CurrentFilter.LastType !== undefined) {
            // Set week, month or whatever
            this.selectedLastTimeType = BarcodesModule.CurrentFilter.LastType;
            this.manualDateRangeSelected = false;
        } 
        else if (BarcodesModule.CurrentFilter.FromDate !== undefined &&
                 BarcodesModule.CurrentFilter.ToDate !== undefined) {
            // Set manual from to date
            this.fromDate = new Date(BarcodesModule.CurrentFilter.FromDate);
            this.toDate = new Date(BarcodesModule.CurrentFilter.ToDate);
            this.manualDateRangeSelected = true;
        }
        else {
            this.fromDate = undefined;
            this.toDate = undefined;
            this.selectedLastTimeType = undefined;
            this.manualDateRangeSelected = false;
        }

        if (BarcodesModule.CurrentFilter.BarcodeFormats !== undefined) {
            this.selectedBarcodeFormats 
                = BarcodesModule.CurrentFilter.BarcodeFormats.map(el => el);
        }
    }
    //#endregion

    //#region OTHER
    getLastTimeTypeString(val: LastTimeType): string {
        return LastTimeTypeToString(val);
    }
    getBarcodeFormatString(val: BarcodeFormat): string {
        return BarcodeFormatToString(val);
    }
    lastTimeTypesEqual(s1: LastTimeType, s2: LastTimeType): boolean {
        if (s1 === undefined || s2 === undefined) return false;
        return s1.valueOf() === s2.valueOf();
    }
    barcodeFormatsEqual(s1: BarcodeFormat, s2: BarcodeFormat): boolean {
        if (s1 === undefined || s2 === undefined) return false;
        return s1.valueOf() === s2.valueOf();
    }
    barcodeFormatContains(s1: BarcodeFormat[], s2: BarcodeFormat): boolean {
        if (s1 === undefined || s2 === undefined || s1.length == 0) return false;

        return s1.indexOf(s2) != -1;
    }
    //#endregion
}
</script>

<style lang="less" scoped>
.filter {
    color: white;
    position: relative;
    // background-color: #2e2e2e;
    background-color: rgb(46, 46, 46);
    border-top-left-radius: 16px;
    border-top-right-radius: 16px;
    padding: 20px;
    pointer-events: all;
    user-select: none;
    width: 100%;
    margin: 0 1.5rem;
    max-width: 450px;
    display: flex;
    flex-direction: column;
    max-height: 75%;

    transition: transform 0.2s ease-out;

    .content {
        overflow-y: auto;
    }

    .close-icon {
        position: absolute;
        right: 0;
        top: 0;
        margin: 20px 25px;
        cursor: pointer;
        font-size: 24px;
    }

    h1 {
        text-align: center;
        margin: 0;
        font-size: 1.75rem;
    }

    .separator {
        height: 3px;
        width: 100%;
        background-color: white;
        margin: 15px 0px;
    }

    .filter-header {
        display: flex;
        font-size: 1.15rem;
        justify-content: space-between;
        margin: 4px 0.75rem;
        
        p {
            margin: 0;
        }

        .desc-asc-switch {
            color: rgb(74, 138, 255);
            font-weight: bold;
            cursor: pointer;
        }

        .reset-button {
            color: rgb(74, 138, 255);
            cursor: pointer;
        }
    }

    .filter-field {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0.75rem 0.65rem;
        cursor: pointer;
        transition: background-color 0.2s ease-out;
        &.selected {
            background-color: rgba(255,255,255, 0.2);
        }
        &:hover {
            background-color: rgba(255, 255, 255, 0.35);
        }
        .check-icon {
            font-size: 18px;
        }
    }

    .available-last-time-types {
        margin: 0.65rem 0.5rem;
    }

    .available-barcode-formats {
        margin: 0.65rem 0.5rem;
    }
}
</style>