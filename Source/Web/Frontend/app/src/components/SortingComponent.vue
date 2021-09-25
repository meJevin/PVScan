<template>
    <div class="sorting">
        <font-awesome-icon class="close-icon" 
        icon="times" color="white" @click="onCloseButtonClicked"/>

        <h1>
            Sort Barcodes
        </h1>

        <div class="separator"></div>

        <div class="sorting-header">
            <p>
                Sort 
                <span class="desc-asc-switch" @click="toggleAscDesc">
                    {{ascDescCaption}}
                </span> 
                by
            </p>

            <p class="reset-button" @click="resetSorting">Reset</p>
        </div>

        <div class="available-sorting-fields">
            <div v-for="field of availableSortingFields" 
                 :key="field" @click="selectSortingField(field)"
                 :class="['sorting-field', sortingFieldsEqual(sortingFieldSelected, field) ? 'selected' : '']">
                 {{getSortingString(field)}}

                <font-awesome-icon v-if="sortingFieldsEqual(sortingFieldSelected, field)" 
                    class="check-icon" icon="check" color="white"/>
            </div>
        </div>

        <button :class="['primary-button medium', CurrentSortingSameAsState ? 'disabled' : '']" 
                @click="onApplyButtonClicked">
            Apply
        </button>
    </div>
</template>

<script lang="ts">
import { Component, Emit, Prop, Vue } from "vue-property-decorator";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { SortingField, Sorting, DefaultSorting, SortingFieldToString } from "../models/Sorting";
import BarcodesModule from "../store/modules/BarcodesModule";

@Component({
    components: {
        FontAwesomeIcon,
    },
})
export default class SortingComponent extends Vue {
    availableSortingFields: SortingField[] = [
        SortingField.Date,
        SortingField.Text,
        SortingField.Format
    ];

    isDesc: boolean = true;
    sortingFieldSelected: SortingField = SortingField.Date;

    get CurrentSortingSameAsState(): boolean {
        // Basically deep comparison...
        console.log("?");
        const currentSorting = JSON.stringify(BarcodesModule.CurrentSorting);
        const stateSorting = JSON.stringify(this.StateSorting);

        return currentSorting === stateSorting;
    }

    get StateSorting(): Sorting {
        const stateSorting: Sorting = {
            Descending: this.isDesc, 
            Field: this.sortingFieldSelected,
        }
        return stateSorting;
    }

    toggleAscDesc() {
        this.isDesc = !this.isDesc;
    }

    selectSortingField(newVal: SortingField) {
        this.sortingFieldSelected = newVal;
    }

    resetSorting() {
        const defaultSorting = DefaultSorting();

        this.isDesc = defaultSorting.Descending;
        this.sortingFieldSelected = defaultSorting.Field;
    }

    //#region EVENTS
    @Emit("closed")
    onCloseButtonClicked() {

    }

    @Emit("sorting-applied")
    onApplyButtonClicked() {
        return this.StateSorting;
    }
    //#endregion

    //#region HOOKS
    created() {
        this.initializeFromModule();
    }

    public initializeFromModule() {
        this.isDesc = BarcodesModule.CurrentSorting.Descending;
        this.sortingFieldSelected = BarcodesModule.CurrentSorting.Field;
    }
    //#endregion

    //#region OTHER 
    getSortingString(val: SortingField): string {
        return SortingFieldToString(val);
    }

    get ascDescCaption(): string {
        return this.isDesc ? "descendingly" : "ascendingly";
    }

    sortingFieldsEqual(s1: SortingField, s2: SortingField): boolean {
        if (s1 === undefined || s2 === undefined) return false;
        return s1.valueOf() === s2.valueOf();
    }
    //#endregion
}
</script>

<style lang="less" scoped>
.sorting {
    color: white;
    position: relative;
    // background-color: #2e2e2e;
    background-color: rgb(46, 46, 46);
    border-top-left-radius: 16px;
    border-top-right-radius: 16px;
    padding: 20px;
    pointer-events: all;
    user-select: none;
    width: 100%;
    margin: 0 1.5rem;
    max-width: 450px;
    display: flex;
    flex-direction: column;

    transition: transform 0.2s ease-out;

    .close-icon {
        position: absolute;
        right: 0;
        top: 0;
        margin: 20px 25px;
        cursor: pointer;
        font-size: 24px;
    }

    h1 {
        text-align: center;
        margin: 0;
        font-size: 1.75rem;
    }

    .separator {
        height: 3px;
        width: 100%;
        background-color: white;
        margin: 15px 0px;
    }

    .sorting-header {
        display: flex;
        font-size: 1.15rem;
        justify-content: space-between;
        margin: 4px 0.75rem;
        
        p {
            margin: 0;
        }

        .desc-asc-switch {
            color: rgb(74, 138, 255);
            font-weight: bold;
            cursor: pointer;
        }

        .reset-button {
            color: rgb(74, 138, 255);
            cursor: pointer;
        }
    }

    .available-sorting-fields {
        margin: 0.65rem 0.5rem;

        .sorting-field {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 0.75rem 0.65rem;
            cursor: pointer;
            transition: background-color 0.2s ease-out;

            &.selected {
                background-color: rgba(255,255,255, 0.2);
            }

            &:hover {
                background-color: rgba(255, 255, 255, 0.35);
            }

            .check-icon {
                font-size: 18px;
            }
        }
    }
}
</style>