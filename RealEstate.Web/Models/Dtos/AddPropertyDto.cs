using Microsoft.AspNetCore.Mvc;

namespace RealEstate.Web.Models.Dtos
{
    public class AddPropertyDto
    {
        public PropertyDto Property { get; set; }
        public string CurrentUserId { get; set; }
        public string CurrentUserRole { get; set; }
    }
}