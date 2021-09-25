<template>
    <div class="history" :style="{ width: PanelWidth }">
        <div class="content">
            <div class="search-bar">
                <font-awesome-icon icon="search" color="rgb(163, 163, 163)" />
                <input type="text" placeholder="Search" class="search-input"/>
                <font-awesome-icon icon="filter" color="rgb(163, 163, 163)" />
            </div>

            <div class="buttons-bar">
                <button
                    :class="[!IsEditingHistoryList ? 'visible' : 'hidden']"
                    @click="handleEditButtonClick"
                >
                    Edit
                </button>
                <button
                    :class="[IsEditingHistoryList ? 'visible' : 'hidden']"
                    style="position: absolute; left: 0px"
                    @click="handleDoneButtonClick"
                >
                    Done
                </button>
                <button
                    :class="DeleteButtonClass"
                    @click="handleDeleteButtonClick"
                >
                    Delete
                </button>
            </div>

            <virtual-list
                class="barcodes-list"
                :data-sources="BarcodesPaged"
                :data-key="
                    (b) => {
                        return b.GUID;
                    }
                "
                :data-component="BarcodeItem"
                :estimate-size="55"
                @tobottom="LoadNextPage"
            >
            </virtual-list>

            <div class="sorting-filter-buttons-container">
                <button class="sorting-button primary-button"
                        @click="handleSortingShow">
                    Sorting
                </button>
                <button class="filter-button primary-button"
                        @click="handleFilterShow">
                    Filter
                </button>
            </div>

            <div class="filter-and-sorting-container">
                <div :class="['overlay', isFilterShown ? '' : 'hidden']" 
                     :style="{opacity: FilterComponentOverlayOpacity}"
                     @click="handleFilterOverlayClicked"/>

                <div class="filter-container">
                    <filter-component ref="filterComponent" 
                        :style="{transform: FilterComponentTranslateY}"
                        @closed="handleFilterClosed"
                        @filter-applied="handleFilterApplied"/>
                </div>

                <div :class="['overlay', isSortingShown ? '' : 'hidden']" 
                     :style="{opacity: SortingComponmentOverlayOpacity}"
                     @click="handleSortingOverlayClicked"/>

                <div class="sorting-container">
                    <sorting-component ref="sortingComponent" 
                        :style="{transform: SortingComponentTranslateY}"
                        @closed="handleSortingClosed"
                        @sorting-applied="handleSortingApplied"/>
                </div>
            </div>
        </div>

        <div class="splitter" v-on:mousedown="handleSplitterMouseDown"></div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

import BarcodeListItem from "./primitive/BarcodeListItem.vue";
import FilterComponent from "./FilterComponent.vue";
import SortingComponent from "./SortingComponent.vue";

import VirtualList from "vue-virtual-scroll-list";

import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import Barcode from "../models/Barcode";

import BarcodesModule from "../store/modules/BarcodesModule";
import UIStateModule from "../store/modules/UIStateModule";
import { Sorting } from "../models/Sorting";
import { Filter } from "../models/Filter";

@Component({
    components: {
        FilterComponent,
        SortingComponent,
        FontAwesomeIcon,
        BarcodeListItem,
        VirtualList,
    },
})
export default class HistoryComponent extends Vue {
    @Prop({ default: 450 })
    panelWidth: number;

    @Prop({ default: false })
    isBeingDragged: boolean;

    //#region BARCODES
    async LoadNextPage() {
        await BarcodesModule.LoadNextPage();
    }

    get DeleteButtonClass(): string {
        let result = "visible";

        if (!this.IsEditingHistoryList) {
            return "hidden";
        }

        if (BarcodesModule.SelectedBarcodes.length <= 0) {
            result += " disabled";
        }

        return result;
    }

    get PanelWidth(): string {
        return this.panelWidth + "px";
    }

    get Barcodes(): Barcode[] {
        return BarcodesModule.Barcodes;
    }

    get BarcodesPaged(): Barcode[] {
        return BarcodesModule.BarcodesPaged;
    }

    get IsEditingHistoryList(): boolean {
        return UIStateModule.UIState.MainView.isEditingHistoryList;
    }

    async handleEditButtonClick() {
        UIStateModule.ToggleHistoryListEdit();
    }

    async handleDoneButtonClick() {
        UIStateModule.ToggleHistoryListEdit();
        BarcodesModule.ClearSelectedBarcodes();
    }

    async handleDeleteButtonClick() {
        await BarcodesModule.DeleteSelectedBarcodes();
        UIStateModule.ToggleHistoryListEdit();
    }
    //#endregion

    //#region SORTING
    isSortingShown: boolean = false;
    sortingComponentHeight: number = 0;

    get SortingComponentTranslateY(): string {
        let yValue = this.sortingComponentHeight;

        if (this.isSortingShown) {
            yValue = 0;
        }

        return `translateY(${yValue}px)`;
    }

    get SortingComponmentOverlayOpacity(): number {
        if (this.isSortingShown) {
            return 0.75;
        }

        return 0;
    }

    async handleSortingShow() {
        if (this.isSortingShown) return;

        (this.$refs.sortingComponent as SortingComponent).initializeFromModule();

        this.isSortingShown = true;
    }

    async handleSortingClosed() {
        if (!this.isSortingShown) return;

        this.isSortingShown = false;
    }

    async handleSortingApplied(newSorting: Sorting) {
        await BarcodesModule.ApplySorting(newSorting);
    }

    async handleSortingOverlayClicked() {
        if (!this.isSortingShown) return;

        this.isSortingShown = false;
    }
    //#endregion

    //#region FILTERING
    isFilterShown: boolean = false;
    filterComponentHeight: number = 0;

    get FilterComponentTranslateY(): string {
        let yValue = this.filterComponentHeight;

        if (this.isFilterShown) {
            yValue = 0;
        }

        return `translateY(${yValue}px)`;
    }

    get FilterComponentOverlayOpacity(): number {
        if (this.isFilterShown) {
            return 0.75;
        }

        return 0;
    }

    async handleFilterShow() {
        if (this.isFilterShown) return;

        (this.$refs.filterComponent as FilterComponent).initializeFromModule();

        this.isFilterShown = true;
    }

    async handleFilterClosed() {
        if (!this.isFilterShown) return;

        this.isFilterShown = false;
    }

    async handleFilterApplied(neFilter: Filter) {
        await BarcodesModule.ApplyFilter(neFilter);
    }

    async handleFilterOverlayClicked() {
        if (!this.isFilterShown) return;

        this.isFilterShown = false;
    }
    //#endregion

    //#region OTHER
    async handleSplitterMouseDown(e: MouseEvent) {
        if (!this.isBeingDragged) {
            this.$emit("start-splitter-drag", e.clientX);
        }
    }

    get BarcodeItem() {
        return BarcodeListItem;
    }

    mounted() {
        let sortingComponent = this.$refs.sortingComponent as Vue;
        let filterComponent = this.$refs.filterComponent as Vue;

        this.sortingComponentHeight = sortingComponent.$el.clientHeight;
        this.filterComponentHeight = filterComponent.$el.clientHeight;
    }
    //#endregion
}
</script>

<style lang="less" scoped>
.history {
    height: 100%;
    display: grid;
    grid-template-columns: 1fr;
    grid-template-rows: 1fr;
    background-color: rgb(46, 46, 46);

    .content {
        grid-row: 1;
        grid-column-start: 1;
        grid-column-end: 3;
        display: flex;
        flex-direction: column;
        overflow: overlay;
        position: relative;
    }

    .splitter {
        grid-row: 1;
        grid-column: 2;
        width: 6px;
        height: 100%;
        cursor: ew-resize;
        background-color: transparent;
        z-index: 1000;
        transform: translateX(3px);
    }

    .search-bar {
        background-color: white;
        display: flex;
        align-items: center;
        padding: 4px 14px;
        border-radius: 6px;

        margin: 12px;

        input {
            flex-grow: 1;
            margin: 0px 6px;
            outline: none;
        }
    }

    .barcodes-list {
        overflow: auto;
        overflow-x: hidden;
        overflow-y: overlay;
    }

    .buttons-bar {
        margin: 4px 12px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        position: relative;
    }

    .sorting-filter-buttons-container {
        position: absolute;
        pointer-events: none;
        width: 100%;
        // background-color: rgba(0,0,0,0.25);
        bottom: 0px;
        display: flex;
        align-content: space-around;
        flex-direction: row;
        flex-wrap: wrap;
        justify-content: center;

        .sorting-button {
            flex-grow: 2;
            margin-left: 40px;
            max-width: 300px;
        }

        .filter-button {
            flex-grow: 1;
            margin-right: 40px;
            max-width: 150px;
        }

        button {
            margin: 20px 10px;
            pointer-events: all;
        }
    }

    .filter-and-sorting-container {
        position: absolute;
        overflow-y: hidden;
        width: 100%;
        height: 100%;
        // background-color: rgba(0,0,0,0.5);
        pointer-events: none;
        z-index: 2000;
    }
}

.overlay {
    position: absolute;
    width: 100%;
    height: 100%;
    background-color: black;
    pointer-events: all;
    transition: opacity 0.2s ease-out;

    &.hidden {
        pointer-events: none;
    }
}

.filter-container {
    position: absolute;
    display: flex;
    align-items: flex-end;
    justify-content: center;
    width: 100%;
    height: 100%;
}

.sorting-container {
    position: absolute;
    display: flex;
    align-items: flex-end;
    justify-content: center;
    width: 100%;
    height: 100%;
}

.search-input {
    border: none;
    color: black;
    padding: 4px;
    height: auto;
}
</style>