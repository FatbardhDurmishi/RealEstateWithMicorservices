namespace RealEstate.Web.Models
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; } = null!;
        public  string Token { get; set; } =null!;
    }
}
