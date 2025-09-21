using System.ComponentModel.DataAnnotations;

namespace Hachiko.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty;

    public string? StreetAddress { get; set; } 
    public string? City { get; set; } 
    public string? State { get; set; } 
    public string? PostalCode { get; set; } 
    [Phone] public string? PhoneNumber { get; set; } 
}