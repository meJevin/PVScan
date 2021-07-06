using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PVScan.Desktop.WPF.Models
{
    public class SQLiteEncrypedKVP
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
