using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RealEstate.Web.Models
{
    public class PropertyViewModel
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
        public int? BedRooms { get; set; }
        public int? BathRooms { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Area { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        [ValidateNever]
        public string Status { get; set; } = null!;

        [StringLength(50)]
        public string State { get; set; } = null!;

        [StringLength(50)]
        public string City { get; set; } = null!;

        [StringLength(50)]
        public string StreetAddress { get; set; } = null!;

        [ValidateNever]
        public string CoverImageUrl { get; set; } = null!;

        public string TransactionType { get; set; } = null!;

        [StringLength(450)]
        public string? UserId { get; set; }

        public int? PropertyTypeId { get; set; }

        public virtual PropertyType? PropertyType { get; set; }

        [ValidateNever]
        public virtual ICollection<PropertyImage> PropertyImages { get; set; }

        [ValidateNever]
        public List<string> BlobUrls { get; set; } = new List<string>();

        [ValidateNever]
        public string CoverImageBlobUrl { get; set; }

        public bool ShowButtons = false;
    }
}