<template>
    <div class="profile"
         :style="{ width: PanelWidth, transform: PanelTransform }">
        <div class="splitter"
            v-on:mousedown="handleSplitterMouseDown"
            v-show="isBeingShown">

        </div>

        <div class="content">
            <div class="login-container">
                <login-component/>
            </div>
            <!-- <div class="sign-up-container">

            </div>
            <div class="logged-in-container">

            </div> -->
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import LoginComponent from "./LoginComponent.vue";

@Component({
    components: {
        LoginComponent: LoginComponent
    }
})
export default class ProfileComponent extends Vue {

    @Prop({ default: 350 })
    panelWidth: number;

    @Prop({default: false})
    isBeingDragged: boolean;

    @Prop({default: false})
    isBeingShown: boolean;

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
    background-color: rgb(46, 46, 46);
    height: 100%;
    display: grid;
    grid-template-columns: 1fr;
    grid-template-rows: 1fr;
    z-index: 3000;
    color: white;

    transition: transform 0.25s ease-out;

    .content {
        grid-row: 1;
        grid-column-start: 1;
        grid-column-end: 3;
        flex-grow: 1;

        .login-container {
            display: flex;
            position: absolute;
            width: 100%;
            height: 100%;
        }
        
        .sign-up-container {
            display: flex;
            position: absolute;
            width: 100%;
            height: 100%;
        }
        
        .logged-in-container {
            display: flex;
            position: absolute;
            width: 100%;
            height: 100%;
        }
    }

    .splitter {
        grid-row: 1;
        grid-column: 1;
        width: 6px;
        height: 100%;
        cursor: ew-resize;
        background-color: transparent;
        transform: translateX(-3px);
    }
}
</style>