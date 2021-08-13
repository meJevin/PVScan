<template>
    <div class="profile">
        <h1>Profile Component</h1>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component
export default class ProfileComponent extends Vue {}
</script>

<style scoped>
.profile {
    position: absolute;
    right: 0px;
    width: 300px;
    background-color: aqua;
    height: 100%;
}
</style>