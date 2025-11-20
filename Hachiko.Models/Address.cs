using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hachiko.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Street Address / Number")]
        public string StreetAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Ward (Phường/Xã)")]
        public string Ward { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "District (Quận/Huyện)")]
        public string District { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "City/Province (Tỉnh/Thành phố)")]
        public string CityProvince { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Required]
        public bool IsDefault { get; set; } = false;

        [MaxLength(50)]
        [Display(Name = "Address Label")]
        public string? Label { get; set; } // e.g., "Home", "Work", "Other"

        // Foreign keys for polymorphic relationship
        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
