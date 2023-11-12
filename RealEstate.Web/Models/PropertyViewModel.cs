using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RealEstate.Web.Models.Dtos;

namespace RealEstate.Web.Models
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Required]

        public string Description { get; set; } = null!;
        [Required]

        public int? BedRooms { get; set; }
        [Required]

        public int? BathRooms { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Area { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Price { get; set; }

        [StringLength(50)]
        [ValidateNever]

        public string Status { get; set; } = null!;
        [Required]


        [StringLength(50)]
        public string State { get; set; } = null!;

        [StringLength(50)]
        [Required]
        public string City { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string StreetAddress { get; set; } = null!;

        [ValidateNever]
        public string CoverImageUrl { get; set; } = null!;

        public string TransactionType { get; set; } = null!;

        [StringLength(450)]
        public string? UserId { get; set; }

        [ValidateNever]
        public UserDto User { get; set; } = null!;
        [Required]

        public int? PropertyTypeId { get; set; }

        public virtual PropertyType? PropertyType { get; set; }

        [ValidateNever]
        public virtual ICollection<PropertyImage> PropertyImages { get; set; }

        [ValidateNever]
        public List<string> BlobUrls { get; set; } = new List<string>();

        [ValidateNever]
        public string CoverImageBlobUrl { get; set; }

        public bool ShowButtons { get; set; } = false;
    }
}