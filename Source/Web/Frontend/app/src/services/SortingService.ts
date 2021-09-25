import Barcode from "@/models/Barcode";
import { Sorting, SortingField } from "@/models/Sorting";
import ISortingService from "./interfaces/ISortingService";

export default class SortingService implements ISortingService {

    private compareDesc(a, b) {
        if (a < b){
          return -1;
        }
        if (a > b){
          return 1;
        }
        return 0;
    }

    private compareAsc(a, b) {
        if (a < b){
          return 1;
        }
        if (a > b){
          return -1;
        }
        return 0;
    }

    async Sort(barcodes: Barcode[], sorting: Sorting): Promise<Barcode[]> {
        if (sorting.Field == SortingField.None) {
            return barcodes;
        }

        let sortedBarcodes = barcodes.map(b => b);
        let compareFunc = sorting.Descending ? this.compareDesc : this.compareAsc;

        return new Promise<Barcode[]>((resolve, reject) => {
            sortedBarcodes.sort((bFirst, bSecond) => {
                if (sorting.Field == SortingField.Date) {
                    return compareFunc(bFirst.ScanTime, bSecond.ScanTime);
                }
                else if (sorting.Field == SortingField.Text) {
                    return compareFunc(bFirst.Text, bSecond.Text);
                }
                else if (sorting.Field == SortingField.Format) {
                    return compareFunc(bFirst.BarcodeFormat, bSecond.BarcodeFormat);
                }

                return 0;
            });

            resolve(sortedBarcodes);
        });
    }
}