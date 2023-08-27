using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.AuthAPI.Models.Dto
{
    public class LoginDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
