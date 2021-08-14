import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';

@Module({dynamic: true, name: "Scanning", store: store})
export class ScanningModule extends VuexModule {
    
}

const ScanningModuleExported = getModule(ScanningModule);

export default ScanningModuleExported;