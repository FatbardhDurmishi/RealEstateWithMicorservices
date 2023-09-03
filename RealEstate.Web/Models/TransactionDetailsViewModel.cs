using RealEstate.Web.Models.Dtos;

namespace RealEstate.Web.Models
{
    public class TransactionDetailsViewModel
    {
        public TransactionViewModel Transaction { get; set; } = null!;
        public PropertyViewModel Property { get; set; } = null!;
        public UserDto Buyer { get; set; } = null!;
        public UserDto Owner { get; set; } = null!;
    }
}