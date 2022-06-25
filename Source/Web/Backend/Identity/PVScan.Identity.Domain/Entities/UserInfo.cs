using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Identity.Domain.Entities
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public int BarcodesScanned { get; set; }
        public int BarcodeFormatsScanned { get; set; }
        public double Experience { get; set; }
        public int Level { get; set; }
        public string? VKLink { get; set; }
        public string? IGLink { get; set; }
    }
}
