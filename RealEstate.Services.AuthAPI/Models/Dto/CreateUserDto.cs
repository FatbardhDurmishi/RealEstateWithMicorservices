namespace RealEstate.Services.AuthAPI.Models.Dto
{
    public class CreateUserDto
    {
        public RegisterDto User { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
