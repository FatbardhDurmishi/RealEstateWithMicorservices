namespace RealEstate.Services.PropertyService.Models.Dtos
{
    public class AddPropertyDto
    {
        public Property Property { get; set; }
        public IFormFile CoverImage { get; set; }
        public IFormFileCollection PropertyImages { get; set; }
    }
}