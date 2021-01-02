using System;
using System.Collections.Generic;
using System.Text;

namespace PVScan.Domain.Models
{
    /// <summary>
    /// App user 
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        /// <summary>
        /// From extenal provider?
        /// </summary>
        public bool IsExternal { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }
    }
}
