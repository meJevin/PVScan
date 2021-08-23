import Barcode from "@/models/Barcode";
import { Filter } from "@/models/Filter";

export default interface IFilterService {
    Filter(barcodes: Barcode[], filter: Filter): Barcode[];
}