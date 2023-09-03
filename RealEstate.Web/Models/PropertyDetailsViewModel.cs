using RealEstate.Web.Models.Dtos;

namespace RealEstate.Web.Models
{
    public class PropertyDetailsViewModel
    {
        public PropertyViewModel Property { get; set; }
        public UserDto User { get; set; }
    }
}