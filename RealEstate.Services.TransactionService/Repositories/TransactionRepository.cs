using Microsoft.EntityFrameworkCore;
using RealEstate.Services.TransactionService.Constants;
using RealEstate.Services.TransactionService.Data;
using RealEstate.Services.TransactionService.Models;
using RealEstate.Services.TransactionService.Repositories.IRepositories;

namespace RealEstate.Services.TransactionService.Repositories
{
    public class TransactionRepository:Repository<Transaction>,ITransactionRepository
    {
        protected readonly AppDbContext _db;
        public TransactionRepository(AppDbContext db):base(db) 
        { 
            _db = db;
        }

        //public void CheckTransactionDate()
        //{
        //    var transactions = _db.Transactions.Include(x => x.TransactionTypeNavigation).Where(x => x.TransactionTypeNavigation.Name == TransactionTypes.Rent).ToList();
        //    if (transactions != null)
        //    {

        //        foreach (var transaction in transactions)
        //        {
        //            if (transaction.RentEndDate < DateTime.Now && transaction.Status != TransactionStatus.Expired)
        //            {
        //                transaction.Status = TransactionStatus.Expired;
        //                var property = _db.Properties.Find(transaction.PropertyId);
        //                if (property != null)
        //                {
        //                    property.Status = PropertyStatus.Free;
        //                    _db.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //}

        public string UpdateStatus(Transaction transaction, string status)
        {
            transaction.Status = status;
            return status;
        }
    }
}
