<template>
    <div class="barcode-list-item" v-if="barcode != null">
        <div class="main-info">
            <p>{{barcode.Text}}</p>
            <span :style="{ opacity: 0.5 }"> {{BarcodeDate}} </span>
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
import {BarcodeFormatToString} from "../../models/Utils";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import moment from "moment";

import BarcodesModule from "../../store/modules/BarcodesModule";

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
        return BarcodeFormatToString(this.barcode);
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
        margin-bottom: 4px;
        font-size: 15px;
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