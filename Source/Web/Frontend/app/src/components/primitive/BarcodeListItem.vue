<template>
    <div class="barcode-list-item" v-if="source != null"
        @click="HandleListItemClick">
        <div class="main-info">
            <p>{{source.Text}}</p>
            <span :style="{ opacity: 0.5 }"> {{BarcodeDate}} </span>
            <span>, {{BarcodeFormat}}</span>
        </div>

        <div class="right-container">
            <div 
            :style="{ opacity: FavoriteIconOpacity }"
            id="fav-icon" :class="FavoriteIconClass"
            @click="FavoriteClicked">
                <font-awesome-icon icon="heart" color="white"/>
            </div>

            <div
                id="check-icon"
                :style="{ opacity: CheckIconOpacity }"> 
                <font-awesome-icon icon="check" color="white"/>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Barcode from "../../models/Barcode";
import {BarcodeFormatToString} from "../../models/Utils";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import moment from "moment";

import BarcodesModule from "../../store/modules/BarcodesModule";
import UIStateModule from "../../store/modules/UIStateModule";

@Component({
    components: {
        FontAwesomeIcon,
    }
})
export default class HistoryComponent extends Vue {

    @Prop()
    index: number;

    @Prop()
    source: Barcode;

    get IsSelected(): boolean {
        return (BarcodesModule.SelectedBarcodes.findIndex(b => b == this.source) != -1);
    }

    get FavoriteIconOpacity(): number {
        if (this.source.Favorite) {
            return 1;
        }

        return 0.5;
    }

    get BarcodeDate(): string {
        return moment(this.source.ScanTime).format('DD/MM/YYYY HH:MM:SS');
    }

    get BarcodeFormat(): string {
        return BarcodeFormatToString(this.source.BarcodeFormat);
    }

    get IsEditing(): boolean {
        return UIStateModule.UIState.MainView.isEditingHistoryList;
    }

    get FavoriteIconClass(): string {
        if (UIStateModule.UIState.MainView.isEditingHistoryList) {
            return "hidden";
        }

        return "";
    }

    get CheckIconOpacity(): number {
        if (this.IsSelected) {
            return 1;
        }

        return 0;
    }

    FavoriteClicked() {
        BarcodesModule.ToggleBarcodeFavorite(this.source);
    }

    HandleListItemClick(e: MouseEvent) {
        if (UIStateModule.UIState.MainView.isEditingHistoryList) {
            if (e.shiftKey) {
                BarcodesModule.SelectBarcodesShiftClick(this.source);

                return;
            }

            if (!this.IsSelected) {
                BarcodesModule.SelectBarcode(this.source);
            }
            else {
                BarcodesModule.DeselectBarcode(this.source);
            }
        }
    }
}
</script>

<style lang="less">
.barcode-list-item {
    padding: 8px 14px;
    background-color: transparent;
    color: white;
    display: flex;
    align-items: center;
    user-select: none;
    
    .main-info {
        flex-grow: 1;
    }

    &:hover {
        background-color: rgb(121, 121, 121);
    }

    transition: all 0.25s ease-in;

    p {
        padding: 0;
        margin: 0;
        margin-bottom: 4px;
        font-size: 15px;
    }
    
    span {
        font-size: 13px;
    }

    .right-container {
        position: relative;

        #fav-icon {
            cursor: pointer;

            &.hidden {
                transform: translateX(45px);
            }

            transition: all 0.25s ease-out;
        }

        #check-icon {
            position: absolute;
            left: 0;
            top: 0;

            transition: all 0.25s ease-out;

            pointer-events: none;
        }

        margin: 0 6px;
    }
}
</style>