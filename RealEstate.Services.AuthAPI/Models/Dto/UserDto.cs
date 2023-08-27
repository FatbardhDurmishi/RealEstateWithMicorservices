namespace RealEstate.Services.AuthAPI.Models.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; }
        public string? StreetAddres { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
    }
}
