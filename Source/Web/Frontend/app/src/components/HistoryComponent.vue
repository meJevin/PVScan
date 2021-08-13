<template>
    <div class="history"
         :style="{ width: SplitterWidth }">
        <div class="content">
            <h1>History Component</h1>
        </div>

        <div class="splitter" 
            v-on:mousedown="handleSplitterMouseDown"
            v-on:mouseup="handleSplitterMouseUp"
            v-on:mousemove="handleSplitterMouseMove"
            >

        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component
export default class HistoryComponent extends Vue {

    private prevClientX: number = -1;
    private isDraggingSplitter: boolean = false;

    private splitterWidth: number = 350;
    
    handleSplitterMouseDown(e: MouseEvent) {
        this.prevClientX = e.clientX;
        this.isDraggingSplitter = true;
    }

    handleSplitterMouseUp(e: MouseEvent) {
        this.isDraggingSplitter = false;
    }

    handleSplitterMouseMove(e: MouseEvent) {
        if (this.isDraggingSplitter) {
            const delta = e.clientX - this.prevClientX;

            this.splitterWidth += delta;

            this.prevClientX = e.clientX;
        }
    }

    get SplitterWidth() {
        return this.splitterWidth + "px";
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
        width: 5px;
        height: 100%;
        background-color: transparent;
        cursor: ew-resize;
    }
}
</style>