namespace RealEstate.Web.Models
{
    public class TransactionListViewModel
    {
        public int Id { get; set; }
        public string OwnerName { get; set; } = null!;
        public string BuyerName { get; set; } = null!;
        public decimal RentPrice { get; set; }
        public string PropertyName { get; set; } = null!;
        public string TransactionType { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public DateTime Date { get; set; }
        public bool ShowButtons { get; set; } = false;
    }
}