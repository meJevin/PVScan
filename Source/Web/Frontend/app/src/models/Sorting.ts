export enum SortingField {
    None,
    Date,
    Text,
    Format
}

export interface Sorting {
    Field: SortingField;
    Descending: boolean;
}

export function IsDefaultSorting(sorting: Sorting): boolean {
    return (sorting.Descending &&
            sorting.Field == SortingField.Date);
}

export function DefaultSorting(): Sorting {
    return {
        Descending: true,
        Field: SortingField.Date,
    };
}

export function SortingFieldToString(sf: SortingField): string {
    return SortingField[sf];
}