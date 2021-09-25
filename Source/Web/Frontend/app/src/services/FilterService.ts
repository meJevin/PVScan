import Barcode from "@/models/Barcode";
import { Filter, IsEmptyFilter, LastTimeType } from "@/models/Filter";
import IFilterService from "./interfaces/IFilterService";

export default class FilterService implements IFilterService {
    async Filter(barcodes: Barcode[], filter: Filter): Promise<Barcode[]> {
        console.log("?");
        if (IsEmptyFilter(filter)) {
            return barcodes;
        }

        let dateFrom = new Date();
        let dateTo = new Date();

        if (filter.LastType) {
            // To - tommorow without hours
            dateTo = new Date();
            dateTo = new Date(dateTo.setHours(0, 0, 0, 0));
            dateTo.setDate(dateTo.getDate() + 1);

            // From - start with today without hours
            dateFrom = new Date();
            dateFrom = new Date(dateFrom.setHours(0, 0, 0, 0));

            // And add days accordingly
            if (filter.LastType === LastTimeType.Day) {
                dateFrom.setDate(dateFrom.getDate() - 1);
            }
            else if (filter.LastType === LastTimeType.Week) {
                dateFrom.setDate(dateFrom.getDate() - 7);
            }
            else if (filter.LastType === LastTimeType.Month) {
                dateFrom.setDate(dateFrom.getDate() - 31);
            }
            else if (filter.LastType === LastTimeType.Year) {
                dateFrom.setDate(dateFrom.getDate() - 356);
            }
        }
        else if (filter.FromDate && filter.ToDate) {
            dateFrom = filter.FromDate;
            dateTo = filter.ToDate;
        }
        
        return new Promise<Barcode[]>((resolve, reject) => {
            let filteredBarcodes = barcodes.filter(b => {
                // Filter by formats
                if (filter.BarcodeFormats && filter.BarcodeFormats.length > 0) {
                    if (filter.BarcodeFormats.indexOf(b.BarcodeFormat) == -1) {
                        return false;
                    }
                }

                // Filter by date
                if (!(b.ScanTime >= dateFrom && b.ScanTime <= dateTo)) {
                    return false;
                }

                return true;
            });

            resolve(filteredBarcodes);
        });
    }

}