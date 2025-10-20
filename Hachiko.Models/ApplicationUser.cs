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

        //TODO: Modify Address Fields matching Vietnamese Address Format
        //TODO: Xử lý địa chủ cũ và địa chỉ mới sau xác nhập
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }  
        public string? PostalCode { get; set; }
    }
}
