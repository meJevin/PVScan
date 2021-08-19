import {Action, getModule, Module, Mutation, VuexModule} from "vuex-module-decorators";
import store from '@/store';

const MIN_PROFILE_WIDTH = 300;
const MAX_PROFILE_WIDTH = 600;

const MIN_HISTORY_WIDTH = 300;
const MAX_HISTORY_WIDTH = 800;

interface MainViewUIState {
    profilePageVisible: boolean;
    
    lastMouseX: number;

    isDraggingProfile: boolean;
    isDraggingHistory: boolean;

    profileWidth: number;
    historyWidth: number;

    isEditingHistoryList: boolean;
}

interface UIState {
    MainView: MainViewUIState;
}

class UIStateConstants {
    public static HistoryWidthKey: string = "HistoryWidth";
    public static ProfileWidthKey: string = "ProfileWidth";
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
            historyWidth: 450,

            isEditingHistoryList: false,
        }
    }

    @Mutation
    SetHistoryWidth(newWidth: number) {
        this.UIState.MainView.historyWidth = newWidth;
        localStorage.setItem(UIStateConstants.HistoryWidthKey,
            this.UIState.MainView.historyWidth.toString());
    }

    @Mutation
    SetProfileWidth(newWidth: number) {
        this.UIState.MainView.profileWidth = newWidth;
        localStorage.setItem(UIStateConstants.ProfileWidthKey,
            this.UIState.MainView.historyWidth.toString());
    }

    @Mutation
    SetLastMouseX(newX: number) {
        this.UIState.MainView.lastMouseX = newX;
    }

    @Mutation
    SetIsDraggingHistory(newVal: boolean) {
        this.UIState.MainView.isDraggingHistory = newVal;
    }

    @Mutation
    SetIsDraggingProfile(newVal: boolean) {
        this.UIState.MainView.isDraggingProfile = newVal;
    }

    @Action
    async HandleMouseMoveMain(e: MouseEvent) {
        const delta = e.clientX - this.UIState.MainView.lastMouseX;

        if (this.UIState.MainView.isDraggingHistory) {
            const resultWidth = this.UIState.MainView.historyWidth + delta;

            if (resultWidth >= MIN_HISTORY_WIDTH &&
                resultWidth <= MAX_HISTORY_WIDTH) {
                this.SetHistoryWidth(resultWidth);
                this.SetLastMouseX(e.clientX);
            }
        }
        else if (this.UIState.MainView.isDraggingProfile) {
            const resultWidth = this.UIState.MainView.profileWidth - delta;

            if (resultWidth >= MIN_PROFILE_WIDTH &&
                resultWidth <= MAX_PROFILE_WIDTH) {
                this.SetProfileWidth(resultWidth);
                this.SetLastMouseX(e.clientX);
            }
        }
    }

    @Action
    async HandleMouseUp(e: MouseEvent) {
        if (this.UIState.MainView.isDraggingHistory) {
            this.SetIsDraggingHistory(false);
        }

        if (this.UIState.MainView.isDraggingProfile) {
            this.SetIsDraggingProfile(false);
        }
    }

    @Action
    async HandleProfileStartDragging(mouseX: number) {
        this.SetIsDraggingProfile(true);
        this.SetLastMouseX(mouseX);
    }

    @Action
    async HandleProfileStopDragging() {
        this.SetIsDraggingProfile(false);
    }

    @Action
    async HandleHistoryStartDragging(mouseX: number) {
        this.SetIsDraggingHistory(true);
        this.SetLastMouseX(mouseX);
    }

    @Action
    async HandleHistoryStopDragging() {
        this.SetIsDraggingHistory(false);
    }

    @Action
    async Initialize() {
        let historyWidth = localStorage.getItem(UIStateConstants.HistoryWidthKey);
        let profileWidth = localStorage.getItem(UIStateConstants.ProfileWidthKey);

        if (historyWidth != null) {
            this.SetHistoryWidth(parseInt(historyWidth));
        }

        if (profileWidth != null) {
            this.SetProfileWidth(parseInt(profileWidth));
        }
    }

    @Mutation
    ToggleProfilePage() {
        this.UIState.MainView.profilePageVisible = !this.UIState.MainView.profilePageVisible;
    }

    @Mutation
    ToggleHistoryListEdit() {
        this.UIState.MainView.isEditingHistoryList = !this.UIState.MainView.isEditingHistoryList;
    }
}

const UIStateModulExported = getModule(UIStateModule);

export default UIStateModulExported;