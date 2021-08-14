<template>
    <div class="barcode-list-item" v-if="barcode != null">
        <div class="main-info">
            <p>{{barcode.Text}}</p>
            <span> {{BarcodeDate}} </span>
            <span>, {{BarcodeFormat}}</span>
        </div>

        <div class="favorite-container"
            @click="FavoriteClicked">
            <font-awesome-icon icon="heart" color="white"
            :style="{ opacity: FavoriteIconOpacity }"/>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Barcode from "../../models/Barcode";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import moment from "moment";

import BarcodesModule from "../../store/modules/BarcodesModule";
import BarcodeFormat from "zxing-typescript/src/core/BarcodeFormat";

@Component({
    components: {
        FontAwesomeIcon,
    }
})
export default class HistoryComponent extends Vue {

    @Prop({required: true})
    barcode!: Barcode;

    get FavoriteIconOpacity(): number {
        if (this.barcode.Favorite) {
            return 1;
        }

        return 0.5;
    }

    get BarcodeDate(): string {
        return moment(this.barcode.ScanTime).format('DD/MM/YYYY HH:MM:SS');
    }

    get BarcodeFormat(): string {
        if (this.barcode.BarcodeFormat == 0) {
            return "AZTEC";
        }
        else if (this.barcode.BarcodeFormat == 11) {
            return "QR Code";
        }
        else if (this.barcode.BarcodeFormat == 10) {
            return "PDF-417";
        }

        return "?";
    }

    FavoriteClicked() {
        BarcodesModule.ToggleBarcodeFavorite(this.barcode);
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
        font-size: 16px;
    }
    
    span {
        font-size: 13px;
    }

    .favorite-container {
        cursor: pointer;
        margin: 0 6px;
    }
}
</style>