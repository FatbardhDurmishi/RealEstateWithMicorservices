namespace RealEstate.Web.Models.Dtos
{
    public class AddTransactionDto
    {
        public int PropertyId { get; set; }
        public string BuyerId { get; set; }
        public string OwnerId { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public string TransactionType { get; set; }
        public decimal PropertyPrice { get; set; }
    }
}