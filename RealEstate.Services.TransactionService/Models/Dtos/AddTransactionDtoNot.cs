using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RealEstate.Services.TransactionService.Models.Dtos
{
    public class AddTransactionDtoNot
    {
        public Transaction Transaction { get; set; } = null!;
        public Property Property { get; set; } = null!;
    }
}