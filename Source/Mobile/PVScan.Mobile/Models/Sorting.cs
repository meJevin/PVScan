using System;
namespace PVScan.Mobile.Models
{
    public enum SortingField
    {
        None,
        Date,
        Text,
        Format
    };

    public class Sorting
    {
        public SortingField Field { get; set; }
        public bool Descending { get; set; }

        public static Sorting Default() => new Sorting()
        {
            Field = SortingField.Date,
            Descending = true,
        };
    }
}
