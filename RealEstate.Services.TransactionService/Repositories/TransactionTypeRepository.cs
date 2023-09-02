using RealEstate.Services.TransactionService.Data;
using RealEstate.Services.TransactionService.Models;
using RealEstate.Services.TransactionService.Repositories.IRepositories;

namespace RealEstate.Services.TransactionService.Repositories
{
    public class TransactionTypeRepository:Repository<TransactionType>,ITransactionTypeRepository
    {
        protected readonly AppDbContext _db;
        public TransactionTypeRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
