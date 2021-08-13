<template>
    <div class="main">
        <history-component/>

        <div class="map_or_scanning">
            <scanning-component/>
            <map-component/>
        </div>

        <profile-component/>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import HistoryComponent from "../components/HistoryComponent.vue";
import MapComponent from "../components/MapComponent.vue";
import ProfileComponent from "../components/ProfileComponent.vue";
import ScanningComponent from "../components/ScanningComponent.vue";

@Component({
    components: {
        HistoryComponent,
        MapComponent,
        ProfileComponent,
        ScanningComponent
    },
})
export default class MainView extends Vue {}
</script>