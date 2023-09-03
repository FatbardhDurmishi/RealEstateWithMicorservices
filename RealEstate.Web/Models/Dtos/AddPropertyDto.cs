namespace RealEstate.Web.Models.Dtos
{
    public class AddPropertyDto
    {
        public PropertyViewModel Property { get; set; }
        public IFormFile CoverImage { get; set; }
        public IFormFileCollection PropertyImages { get; set; }
    }
}