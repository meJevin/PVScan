using PVScan.Domain.DTO.Barcodes;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.Domain.DTO.Synchronizer
{
    public class SynchronizeResponse
    {
        // Existed on server DB, but are not present localy; need to add them localy
        public IEnumerable<Barcode> ToAddLocaly { get; set; }
        // Existed on server DB, but were last time updated in DB is later than on local machine;
        // need to update them localy
        public IEnumerable<Barcode> ToUpdateLocaly { get; set; }

        // Didn't exist on server DB; need to add them to other clients; returns GUIDs
        public IEnumerable<string> ToAddToServer { get; set; }
        // Existed on server DB, but were last time updated in DB earlier than on loal machine,
        // need to update them on other clients where LastTimeUpdated is earlier; returns GUIDs
        public IEnumerable<string> ToUpdateOnServer { get; set; }
    }
}
