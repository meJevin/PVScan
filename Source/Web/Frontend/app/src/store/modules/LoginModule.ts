import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';

@Module({dynamic: true, name: "Login", store: store})
export class LoginModule extends VuexModule {
    
}

const LoginModuleExported = getModule(LoginModule);

export default LoginModuleExported;