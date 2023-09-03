using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RealEstate.Services.PropertyService.Models.Dtos;

namespace RealEstate.Services.PropertyService.Models
{
    public class Property
    {
        [Key]
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
        public string Status { get; set; } = null!;

        [StringLength(50)]
        public string State { get; set; } = null!;

        [StringLength(50)]
        public string City { get; set; } = null!;

        [StringLength(50)]
        public string StreetAddress { get; set; } = null!;

        public string CoverImageUrl { get; set; } = null!;
        public string TransactionType { get; set; } = null!;

        [StringLength(450)]
        public string? UserId { get; set; }

        public UserDto User { get; set; }

        public int? PropertyTypeId { get; set; }

        [ForeignKey("PropertyTypeId")]
        [InverseProperty("Properties")]
        public virtual PropertyType? PropertyType { get; set; }

        [InverseProperty("Property")]
        public virtual ICollection<PropertyImage> PropertyImages { get; set; }

        [NotMapped]
        public List<string> BlobUrls { get; set; } = new List<string>();

        [NotMapped]
        public string CoverImageBlobUrl { get; set; }

        [NotMapped]
        public bool ShowButtons = false;
    }
}