using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Domain.DTO.Users
{
    public class ChangeRequest
    {
        public string VKLink { get; set; }
        public string IGLink { get; set; }
    }
}
