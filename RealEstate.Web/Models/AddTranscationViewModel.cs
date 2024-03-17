namespace RealEstate.Web.Models
{
    public class AddTransactionViewModel
    {
        public TransactionViewModel Transaction { get; set; } = null!;
        public PropertyViewModel Property { get; set; } = null!;
    }
}