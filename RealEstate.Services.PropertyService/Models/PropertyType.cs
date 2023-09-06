using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace RealEstate.Services.PropertyService.Models
{
    public class PropertyType
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("PropertyType")]
        [ValidateNever]
        public virtual ICollection<Property>? Properties { get; set; }
    }
}