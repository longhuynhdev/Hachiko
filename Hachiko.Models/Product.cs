using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Hachiko.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = String.Empty;
        public string? Description { get; set; }
        public string ISBN { get; set; } = String.Empty;
        [Required]
        public string Author { get; set; } = String.Empty;
        [Required]
        [Display(Name ="List Price")]
        [Range(0,int.MaxValue)]
        public double ListPrice { get; set; }

        [Display(Name = "Price 1")]
        [Range(0, int.MaxValue)]
        public double Price1 { get; set; }

        [Display(Name = "Price 2")]
        [Range(0, int.MaxValue)]
        public double Price2 { get; set; }

        [Display(Name = "Price 3")]
        [Range(0, int.MaxValue)]
        public double Price3 { get; set; }

        public int CategoryId { get; set; }
        //Reference key
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; } 
        [ValidateNever]
        public string? ImageUrl { get; set; } 
    }
}
