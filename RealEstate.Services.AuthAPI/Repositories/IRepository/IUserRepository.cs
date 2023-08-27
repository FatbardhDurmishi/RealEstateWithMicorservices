using RealEstate.Services.AuthAPI.Models;

namespace RealEstate.Services.AuthAPI.Repositories.IRepository
{
    public interface IUserRepository:IRepository<ApplicationUser>
    {
        ApplicationUser? GetByStringId(string id);
    }
}
