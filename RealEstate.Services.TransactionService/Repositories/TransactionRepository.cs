using Microsoft.EntityFrameworkCore;
using RealEstate.Services.TransactionService.Constants;
using RealEstate.Services.TransactionService.Data;
using RealEstate.Services.TransactionService.Models;
using RealEstate.Services.TransactionService.Repositories.IRepositories;

namespace RealEstate.Services.TransactionService.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        protected readonly AppDbContext _db;
        public TransactionRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public string UpdateStatus(Transaction transaction, string status)
        {
            transaction.Status = status;
            return status;
        }
    }
}
