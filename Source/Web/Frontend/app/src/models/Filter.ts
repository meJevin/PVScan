export enum LastTimeType {
    Day,
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

export function IsDefaultFilter(filter: Filter): boolean {
    return (!filter.BarcodeFormats &&
            !filter.FromDate &&
            !filter.LastType &&
            !filter.ToDate);
}

export function DefaultFilter(): Filter {
    return {};
}