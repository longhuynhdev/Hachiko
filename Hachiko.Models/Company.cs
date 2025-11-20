using System.ComponentModel.DataAnnotations;

namespace Hachiko.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

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