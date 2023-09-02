
using RealEstate.Services.TransactionService.Models;

namespace RealEstate.Services.TransactionService.Repositories.IRepositories
{
    public interface ITransactionRepository:IRepository<Transaction>
    {
        string UpdateStatus(Transaction transaction, string status);
        //void CheckTransactionDate();

    }
}
