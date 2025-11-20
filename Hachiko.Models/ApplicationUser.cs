using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachiko.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; } = String.Empty;

        // Navigation property for addresses
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        // Deprecated: Old address fields - will be migrated to Address table
        [Obsolete("Use Addresses collection instead")]
        public string? StreetAddress { get; set; }
        [Obsolete("Use Addresses collection instead")]
        public string? City { get; set; }
        [Obsolete("Use Addresses collection instead")]
        public string? State { get; set; }
        [Obsolete("Use Addresses collection instead")]
        public string? PostalCode { get; set; }
    }
}
