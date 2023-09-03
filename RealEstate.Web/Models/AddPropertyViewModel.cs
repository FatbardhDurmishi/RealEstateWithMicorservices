using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace RealEstate.Web.Models
{
    public class AddPropertyViewModel
    {
        public PropertyViewModel Property { get; set; } = null!;

        [ValidateNever]
        public IEnumerable<SelectListItem> UsersList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> PropertyTypeList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Cities { get; set; }

        [ValidateNever]
        [DisplayName("Choose the cover Image of your property")]
        public IFormFile CoverImage { get; set; }

        [ValidateNever]
        [DisplayName("Choose the Images of your Property")]
        public IFormFileCollection PropertyImages { get; set; }
    }
}