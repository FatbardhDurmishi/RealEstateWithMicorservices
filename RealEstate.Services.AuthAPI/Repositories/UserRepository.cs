using RealEstate.Services.AuthAPI.Data;
using RealEstate.Services.AuthAPI.Models;
using RealEstate.Services.AuthAPI.Repositories.IRepository;

namespace RealEstate.Services.AuthAPI.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        protected readonly AppDbContext _db;

        public UserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public ApplicationUser? GetByStringId(string id)
        {
            return _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
        }

    }
}
