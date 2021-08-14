<template>
    <div class="profile"
         :style="{ width: PanelWidth, transform: PanelTransform }">
        <div class="splitter"
            v-on:mousedown="handleSplitterMouseDown">

        </div>

        <div class="content">
            <h1>Profile Component</h1>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class ProfileComponent extends Vue {

    @Prop({ default: 350 })
    panelWidth: number = 350;

    @Prop({default: false})
    isBeingDragged: boolean = false;

    @Prop({default: false})
    isBeingShown: boolean = false;

    handleSplitterMouseDown(e: MouseEvent) {
        if (!this.isBeingDragged) {
            this.$emit("start-splitter-drag", e.clientX);
        }
    }

    get PanelWidth(): string {
        return this.panelWidth + "px";
    }

    get PanelTransform(): string {

        if (this.isBeingShown) {
            return "translateX(0px)";
        }

        return `translateX(${this.panelWidth}px)`;
    }
}
</script>

<style lang="less" scoped>
.profile {
    position: absolute;
    right: 0px;
    background-color: aqua;
    height: 100%;
    display: flex;

    transition: transform 0.45s ease-in-out;

    .content {
        flex-grow: 1;
    }

    .splitter {
        width: 10px;
        height: 100%;
        cursor: ew-resize;
        background-color: transparent;
        transform: translateX(-5px);
    }
}
</style>