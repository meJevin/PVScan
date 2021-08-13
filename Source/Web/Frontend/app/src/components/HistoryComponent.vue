<template>
    <div class="history"
         :style="{ width: PanelWidth }">
        <div class="content">
            <h1>History Component</h1>
        </div>

        <div class="splitter" 
            v-on:mousedown="handleSplitterMouseDown"
            >

        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class HistoryComponent extends Vue {

    @Prop({ default: 350 })
    panelWidth: number = 350;

    @Prop({default: false})
    isBeingDragged: boolean = false;
    
    handleSplitterMouseDown(e: MouseEvent) {
        if (!this.isBeingDragged) {
            this.$emit("start-splitter-drag", e.clientX);
        }
    }

    get PanelWidth(): string {
        return this.panelWidth + "px";
    }

}
</script>

<style lang="less" scoped>
.history {
    height: 100%;
    display: flex;

    .content {
        flex-grow: 1;
    }

    .splitter {
        width: 10px;
        height: 100%;
        cursor: ew-resize;
        background-color: transparent;
        z-index: 1000;
        transform: translateX(5px);
    }
}
</style>