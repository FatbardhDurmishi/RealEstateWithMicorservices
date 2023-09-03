namespace RealEstate.Web.Models
{
    public class AddTranscationViewModel
    {
        public TransactionViewModel Transaction { get; set; } = null!;
        public PropertyViewModel Property { get; set; } = null!;
    }
}