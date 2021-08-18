<template>
  <div id="app" :style="{'user-select': UserSelect}">
    <router-view/>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import UIStateModule from "./store/modules/UIStateModule";

@Component
export default class App extends Vue {
    get UserSelect(): string {
        if (UIStateModule.UIState.MainView.isDraggingProfile ||
            UIStateModule.UIState.MainView.isDraggingHistory) {
                return "none";    
            }

        return "unset";
    }
}
</script>
