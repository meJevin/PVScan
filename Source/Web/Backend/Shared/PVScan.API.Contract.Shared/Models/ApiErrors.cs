using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.API.Contract.Shared.Models
{
    public class ApiErrors
    {
        public ICollection<ApiError> Errors { get; set; }
    }
}
