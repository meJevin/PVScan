<template>
    <div class="login">
        <input type="text" placeholder="Login" style="margin-bottom: 0.35rem" 
            autocomplete="false"/>
        <input type="password" placeholder="Password" autocomplete="false"/>

        <div class="forgot-password-container">
            <span>Forgot password?</span>
        </div>

        <button class="primary-button medium">Log in</button>

        <div class="login-sign-up-button-separator">
            <div class="line"></div>
            <span>or</span>
            <div class="line"></div>
        </div>

        <button class="medium">Sign up</button>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class LoginComponent extends Vue {
    login: string;
    password: string;

    onLoginClicked() {}

    onSignUpClicked() {}
}
</script>

<style lang="less" scoped>
.login {
    display: flex;
    padding: 1.5rem;
    flex-direction: column;
    align-items: stretch;
    justify-content: center;
    grid-row: 1;
    grid-column-start: 1;
    grid-column-end: 3;
    flex-grow: 1;

    .forgot-password-container {
        display: flex;
        color: orangered;
        margin: 0.75rem 0.5rem;
        font-size: 14px;
        justify-content: flex-end;

        span {
            cursor: pointer;
            letter-spacing: 1px;
        }
    }

    .login-sign-up-button-separator {
        display: flex;
        align-items: center;
        margin: 0.5rem 0;

        .line {
            height: 2px;
            background: white;
            flex: 1;
        }

        span {
            margin: 0 0.25rem;
            font-size: 14px;
        }
    }
}
</style>