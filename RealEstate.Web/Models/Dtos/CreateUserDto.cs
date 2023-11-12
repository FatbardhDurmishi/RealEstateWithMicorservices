namespace RealEstate.Web.Models.Dtos
{
    public class CreateUserDto
    {
        public RegisterViewModel User { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}
