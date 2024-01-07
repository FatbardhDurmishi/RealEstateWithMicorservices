using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Web.Models
{
    public class RegisterViewModel
    {
        [ValidateNever]
        public string Id
        {
            get; set;
        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get; set;
        }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword
        {
            get; set;
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string? OldPassword
        {
            get; set;
        }
        [Required]
        public string Name
        {
            get; set;
        }
        [Required]
        public string? StreetAddres
        {
            get; set;
        }
        [Required]
        public string? City
        {
            get; set;
        }
        [Required]
        public string? State
        {
            get; set;
        }
        [Required]
        public string? PostalCode
        {
            get; set;
        }
        [Required]
        public string? PhoneNumber
        {
            get; set;
        }
        public string? Role
        {
            get; set;
        }
    }
}