using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Web.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int? PropertyId { get; set; }
    }
}