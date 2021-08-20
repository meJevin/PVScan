import Barcode from "@/models/Barcode";
import { Sorting } from "@/models/Sorting";

export default interface ISortingService {
    Sort(barcodes: Barcode[], sorting: Sorting): Barcode[];
}