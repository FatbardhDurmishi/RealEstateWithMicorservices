using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.PropertyService.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int? PropertyId { get; set; }

        [ForeignKey("PropertyId")]
        [InverseProperty("PropertyImages")]
        public virtual Property? Property { get; set; }
    }
}
