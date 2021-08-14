<template>
    <div class="history"
         :style="{ width: PanelWidth }">
        <div class="content">
            <div class="search-bar">
                <font-awesome-icon icon="search" color="black"/>
                <input type="text" placeholder="Search">
                <font-awesome-icon icon="filter" color="black"/>
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
    display: flex;

    .content {
        flex-grow: 1;
        padding-left: 12px;
        padding-top: 12px;
    }

    .splitter {
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
        padding: 8px 14px;
        border-radius: 12px;

        input {
            flex-grow: 1;
            margin: 0px 6px;
            outline: none;
        }
    }
}
</style>