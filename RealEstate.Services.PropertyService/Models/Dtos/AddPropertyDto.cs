namespace RealEstate.Services.PropertyService.Models.Dtos
{
    public class AddPropertyDto
    {
        public PropertyDto Property { get; set; } = null!;
        public string CurrentUserId { get; set; } = null!;
        public string CurrentUserRole { get; set; } = null!;
    }
}