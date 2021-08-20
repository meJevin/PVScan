export enum SortingField {
    None,
    Date,
    Text,
    Format
}

export interface Sorting {
    Filed: SortingField;
    Descending: boolean;
}

export function IsDefaultSorting(sorting: Sorting): boolean {
    return (sorting.Descending &&
            sorting.Filed == SortingField.Date);
}

export function DefaultSorting(): Sorting {
    return {
        Descending: true,
        Filed: SortingField.Date,
    };
}