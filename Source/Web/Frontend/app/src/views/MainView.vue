<template>
    <div class="main"
        @mousemove="handleMouseMoveMain"
        @mouseup="handleMouseUp">

        <history-component :panelWidth="historyWidth"
        :isBeingDragged="isDraggingHistory"
        @start-splitter-drag="handleHistoryStartDragging"
        @stop-splitter-drag="handleHistoryStopDragging"/>

        <div class="map_or_scanning">
            <scanning-component/>
            <map-component/>

        <button @click="toggleProfilePage">Show profile</button>
        </div>

        <profile-component :panelWidth="profileWidth"
        :isBeingDragged="isDraggingProfile"
        @start-splitter-drag="handleProfileStartDragging"
        @stop-splitter-drag="handleProfileStopDragging"
        :isBeingShown="profilePageVisible"/>
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

    profilePageVisible: boolean = false;

    toggleProfilePage() {
        this.profilePageVisible = !this.profilePageVisible;
    }
//#region Dragging
    private lastMouseX: number = -1;

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
        
        if (this.isDraggingProfile) {
            const delta = e.clientX - this.lastMouseX;

            const resultWidth = this.profileWidth - delta;
            if (resultWidth >= this.minProfileWidth &&
                resultWidth <= this.maxProfileWidth) {
                this.profileWidth -= delta;
                this.lastMouseX = e.clientX;
            }
        }
    }

    handleMouseUp(e: MouseEvent) {
        if (this.isDraggingHistory) {
            this.isDraggingHistory = false;
        }

        if (this.isDraggingProfile) {
            this.isDraggingProfile = false;
        }
    }

    private minProfileWidth = 200;
    private maxProfileWidth = 600;

    isDraggingProfile: boolean = false;
    profileWidth: number = 350;

    handleProfileStartDragging(mouseX: number) {
        this.isDraggingProfile = true;
        this.lastMouseX = mouseX;
    }

    handleProfileStopDragging() {
        this.isDraggingProfile = false;
    }
    private minHistoryWidth = 200;
    private maxHistoryWidth = 600;
    isDraggingHistory: boolean = false;
    historyWidth: number = 350;

    handleHistoryStartDragging(mouseX: number) {
        this.isDraggingHistory = true;
        this.lastMouseX = mouseX;
    }

    handleHistoryStopDragging() {
        this.isDraggingHistory = false;
    }
//#endregion
}
</script>