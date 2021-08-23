import Barcode from "@/models/Barcode";
import { Filter, IsEmptyFilter } from "@/models/Filter";
import IFilterService from "./interfaces/IFilterService";

export default class FilterService implements IFilterService {
    Filter(barcodes: Barcode[], filter: Filter): Barcode[] {
        if (IsEmptyFilter(filter)) {
            return barcodes;
        }

        let filteredBarcodes = barcodes.filter(b => {
            if (filter.BarcodeFormats && filter.BarcodeFormats.length > 0) {
                // Filter by formats
            }

            if (filter.FromDate && filter.ToDate) {
                // Filter by date
            }

            if (filter.LastType) {
                // Check if it's in range of week, nonth, year etc
            }
        });

        return [];
    }
    
}