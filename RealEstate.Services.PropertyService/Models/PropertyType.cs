using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.PropertyService.Models
{
    public class PropertyType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("PropertyType")]
        public virtual ICollection<Property> Properties { get; set; }
    }
}
