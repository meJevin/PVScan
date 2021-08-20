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

    Sort(barcodes: Barcode[], sorting: Sorting): Barcode[] {
        if (sorting.Filed == SortingField.None) {
            return barcodes;
        }

        let sortedBarcodes = barcodes.map(b => b);
        let compareFunc = sorting.Descending ? this.compareDesc : this.compareAsc;

        sortedBarcodes.sort((bFirst, bSecond) => {
            if (sorting.Filed == SortingField.Date) {
                return compareFunc(bFirst.ScanTime, bSecond.ScanTime);
            }
            else if (sorting.Filed == SortingField.Text) {
                return compareFunc(bFirst.Text, bSecond.Text);
            }
            else if (sorting.Filed == SortingField.Format) {
                return compareFunc(bFirst.BarcodeFormat, bSecond.BarcodeFormat);
            }

            return 0;
        });

        return sortedBarcodes;
    }
}