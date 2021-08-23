export enum LastTimeType {
    Day = 1,
    Week,
    Month,
    Year,
}

export interface Filter {
    FromDate?: Date;
    ToDate?: Date;
    LastType?: LastTimeType;
    BarcodeFormats?: number[];
}

export function IsEmptyFilter(filter: Filter): boolean {
    return (!filter.BarcodeFormats &&
            !filter.FromDate &&
            !filter.LastType &&
            !filter.ToDate);
}

export function EmptyFilter(): Filter {
    return {};
}