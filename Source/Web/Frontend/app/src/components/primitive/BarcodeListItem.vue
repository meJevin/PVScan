<template>
    <div class="barcode-list-item" v-if="barcode != null">
        <p>{{barcode.Text}}</p>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import Barcode from "../../models/Barcode";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

@Component({
    components: {
        FontAwesomeIcon,
    }
})
export default class HistoryComponent extends Vue {

    @Prop({ default: null })
    barcode: Barcode;

}
</script>

<style lang="less">
.barcode-list-item {
    height: 40px;
    background-color: pink;
    
    p {
        padding: 0;
        margin: 0;
    }
}
</style>