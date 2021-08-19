<template>
    <div class="main"
        @mousemove="handleMouseMoveMain"
        @mouseup="handleMouseUp">

        <history-component :panelWidth="HistoryWidth"
        :isBeingDragged="IsDraggingHistory"
        @start-splitter-drag="handleHistoryStartDragging"
        @stop-splitter-drag="handleHistoryStopDragging"/>

        <div class="map_or_scanning">
            <scanning-component/>
            <map-component/>
        </div>

        <div id="profilePageOverlay"
             :style="{ 
                opacity: ProfilePageOverlayOpacity,
                'pointer-events': ProfilePageOverlayPointerEvents,
             }"
             @click="toggleProfilePage">
        </div>

        <div class="toggle-profile-button"
            @click="toggleProfilePage">
            <font-awesome-icon icon="bars" size="2x" color="white"/>
        </div>

        <profile-component :panelWidth="ProfileWidth"
        :isBeingDragged="IsDraggingProfile"
        @start-splitter-drag="handleProfileStartDragging"
        @stop-splitter-drag="handleProfileStopDragging"
        :isBeingShown="ProfilePageVisible"/>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import HistoryComponent from "../components/HistoryComponent.vue";
import MapComponent from "../components/MapComponent.vue";
import ProfileComponent from "../components/ProfileComponent.vue";
import ScanningComponent from "../components/ScanningComponent.vue";

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import UIStateModule from "../store/modules/UIStateModule";

@Component({
    components: {
        HistoryComponent,
        MapComponent,
        ProfileComponent,
        ScanningComponent,
        FontAwesomeIcon
    },
})
export default class MainView extends Vue {

    get ProfilePageOverlayOpacity(): number {
        if (UIStateModule.UIState.MainView.profilePageVisible) {
            return 0.75;
        }

        return 0;
    }

    get ProfilePageOverlayPointerEvents(): string {
        if (UIStateModule.UIState.MainView.profilePageVisible) {
            return "all";
        }

        return "none";
    }

    get HistoryWidth(): number {
        return UIStateModule.UIState.MainView.historyWidth;
    }

    get IsDraggingHistory(): boolean {
        return UIStateModule.UIState.MainView.isDraggingHistory;
    }

    get ProfileWidth(): number {
        return UIStateModule.UIState.MainView.profileWidth;
    }

    get IsDraggingProfile(): boolean {
        return UIStateModule.UIState.MainView.isDraggingProfile;
    }

    get ProfilePageVisible(): boolean {
        return UIStateModule.UIState.MainView.profilePageVisible;
    }

    toggleProfilePage() {
        UIStateModule.ToggleProfilePage();
    }

    handleMouseMoveMain(e: MouseEvent) {
        if (UIStateModule.UIState.MainView.isDraggingProfile ||
            UIStateModule.UIState.MainView.isDraggingHistory) {
                UIStateModule.HandleMouseMoveMain(e);
        }
    }

    handleMouseUp(e: MouseEvent) {
        UIStateModule.HandleMouseUp(e);
    }

    handleProfileStartDragging(mouseX: number) {
        UIStateModule.HandleProfileStartDragging(mouseX);
    }

    handleProfileStopDragging() {
        UIStateModule.HandleProfileStopDragging();
    }

    handleHistoryStartDragging(mouseX: number) {
        UIStateModule.HandleHistoryStartDragging(mouseX);
    }

    handleHistoryStopDragging() {
        UIStateModule.HandleHistoryStopDragging();
    }
}
</script>

<style lang="less" scoped>
#profilePageOverlay {
    position: absolute;
    width: 100vw;
    height: 100vh;
    background-color: black;
    transition: opacity 0.25s ease-out;
    z-index: 1001;
}

.toggle-profile-button {
    position: absolute;
    right: 0;
    top: 0;
    margin: 20px;
    cursor: pointer;
}
</style>