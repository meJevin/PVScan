<template>
    <div class="history"
         :style="{ width: PanelWidth }">
        <div class="content">
            <div class="search-bar">
                <font-awesome-icon icon="search" color="rgb(163, 163, 163)"/>
                <input type="text" placeholder="Search">
                <font-awesome-icon icon="filter" color="rgb(163, 163, 163)"/>
            </div>

            <div class="buttons-bar">
                <button class="primary-btn">Edit</button>
                <button class="primary-btn">Delete</button>
            </div>

            <div class="barcodes-list">
                <barcode-list-item :barcode="barcode" v-for="barcode in Barcodes" :key="barcode.Id"/>
            </div>
        </div>

        <div class="splitter" 
            v-on:mousedown="handleSplitterMouseDown"
            >
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

import BarcodeListItem from "./primitive/BarcodeListItem.vue";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import Barcode from "../models/Barcode";

import BarcodesModule from "../store/modules/BarcodesModule";


@Component({
    components: {
        FontAwesomeIcon,
        BarcodeListItem,
    }
})
export default class HistoryComponent extends Vue {

    @Prop({ default: 350 })
    panelWidth: number = 350;

    @Prop({default: false})
    isBeingDragged: boolean = false;
    
    handleSplitterMouseDown(e: MouseEvent) {
        if (!this.isBeingDragged) {
            this.$emit("start-splitter-drag", e.clientX);
        }
    }

    get PanelWidth(): string {
        return this.panelWidth + "px";
    }

    get Barcodes(): Barcode[] {
        return BarcodesModule.Barcodes;
    }
}
</script>

<style lang="less" scoped>
.history {
    height: 100%;
    display: grid;
    grid-template-columns: 1fr;
    grid-template-rows: 1fr;
    background-color: rgb(46,46,46);

    .content {
        grid-row: 1;
        grid-column-start: 1;
        grid-column-end: 3;
        display: flex;
        flex-direction: column;
        overflow: overlay;
    }

    .splitter {
        grid-row: 1;
        grid-column: 2;
        width: 10px;
        height: 100%;
        cursor: ew-resize;
        background-color: transparent;
        z-index: 1000;
        transform: translateX(5px);
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
    }

    .buttons-bar {
        margin: 4px 12px;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }
}
</style>