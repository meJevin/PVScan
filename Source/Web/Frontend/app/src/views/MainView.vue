<template>
    <div class="main"
        @mousemove="handleMouseMoveMain"
        @mouseup="handleMouseUp">

        <history-component :panelWidth="historyWidth"
        @start-splitter-drag="handleHistoryStartDragging"
        @stop-splitter-drag="handleHistoryStopDragging"/>

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
export default class MainView extends Vue {

//#region History Component Splitter Drag
    private minHistoryWidth = 200;
    private maxHistoryWidth = 600;
    isDraggingHistory: boolean = false;
    historyWidth: number = 350;
    private lastMouseX: number = -1;

    handleHistoryStartDragging(mouseX: number) {
        this.isDraggingHistory = true;
        this.lastMouseX = mouseX;
    }

    handleHistoryStopDragging() {
        this.isDraggingHistory = false;
    }

    handleMouseMoveMain(e: MouseEvent) {
        if (this.isDraggingHistory) {
            const delta = e.clientX - this.lastMouseX;

            const resultWidth = this.historyWidth + delta;
            if (resultWidth >= this.minHistoryWidth &&
                resultWidth <= this.maxHistoryWidth) {
                this.historyWidth += delta;
                this.lastMouseX = e.clientX;
            }
        }
    }

    handleMouseUp(e: MouseEvent) {
        if (this.isDraggingHistory) {
            this.isDraggingHistory = false;
        }
    }
//#endregion
}
</script>