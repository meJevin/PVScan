<template>
    <div class="barcode-list-item" v-if="barcode != null">
        <h4>{{barcode.Text}}</h4>
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

}
</style>