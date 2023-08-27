using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.AuthAPI.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; } = null!;
        public string? StreetAddres { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? CompanyId { get; set; }
    }
}
