import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';

const MIN_PROFILE_WIDTH = 200;
const MAX_PROFILE_WIDTH = 600;

const MIN_HISTORY_WIDTH = 200;
const MAX_HISTORY_WIDTH = 600;

interface MainViewUIState {
    profilePageVisible: boolean;
    
    lastMouseX: number;

    isDraggingProfile: boolean;
    isDraggingHistory: boolean;

    profileWidth: number;
    historyWidth: number;
}

interface UIState {
    MainView: MainViewUIState;
}

@Module({dynamic: true, name: "UIState", store: store})
export class UIStateModule extends VuexModule {
    UIState: UIState = {
        MainView: {
            profilePageVisible: false,

            lastMouseX: -1,

            isDraggingProfile: false,
            isDraggingHistory: false,
        
            profileWidth: 350,
            historyWidth: 350,
        }
    }

    @Mutation
    HandleMouseMoveMain(e: MouseEvent) {
        if (this.UIState.MainView.isDraggingHistory) {
            const delta = e.clientX - this.UIState.MainView.lastMouseX;

            const resultWidth = this.UIState.MainView.historyWidth + delta;
            if (resultWidth >= MIN_HISTORY_WIDTH &&
                resultWidth <= MAX_HISTORY_WIDTH) {
                this.UIState.MainView.historyWidth += delta;
                this.UIState.MainView.lastMouseX = e.clientX;
            }
        }
        
        if (this.UIState.MainView.isDraggingProfile) {
            const delta = e.clientX - this.UIState.MainView.lastMouseX;

            const resultWidth = this.UIState.MainView.profileWidth - delta;
            if (resultWidth >= MIN_PROFILE_WIDTH &&
                resultWidth <= MAX_PROFILE_WIDTH) {
                this.UIState.MainView.profileWidth -= delta;
                this.UIState.MainView.lastMouseX = e.clientX;
            }
        }
    }

    @Mutation
    HandleMouseUp(e: MouseEvent) {
        if (this.UIState.MainView.isDraggingHistory) {
            this.UIState.MainView.isDraggingHistory = false;
        }

        if (this.UIState.MainView.isDraggingProfile) {
            this.UIState.MainView.isDraggingProfile = false;
        }
    }

    @Mutation
    HandleProfileStartDragging(mouseX: number) {
        this.UIState.MainView.isDraggingProfile = true;
        this.UIState.MainView.lastMouseX = mouseX;
    }

    @Mutation
    HandleProfileStopDragging() {
        this.UIState.MainView.isDraggingProfile = false;
    }

    @Mutation
    HandleHistoryStartDragging(mouseX: number) {
        this.UIState.MainView.isDraggingHistory = true;
        this.UIState.MainView.lastMouseX = mouseX;
    }

    @Mutation
    HandleHistoryStopDragging() {
        this.UIState.MainView.isDraggingHistory = false;
    }

    @Mutation
    ToggleProfilePage() {
        this.UIState.MainView.profilePageVisible = !this.UIState.MainView.profilePageVisible;
    }
}

const UIStateModulExported = getModule(UIStateModule);

export default UIStateModulExported;