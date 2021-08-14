import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';

@Module({dynamic: true, name: "Login", store: store})
export class LoginModule extends VuexModule {
    private name: string = "Michael";

    @Mutation
    SetName(newName: string) {
        this.name = newName;
    }

    get GetName() {
        return this.name;
    }

    @Action
    async ChangeName(newName: string) {
        setTimeout(() => {
            this.SetName(newName);
        }, 1500);
    }
}

const LoginModuleExported = getModule(LoginModule);

export default LoginModuleExported;