using RealEstate.Services.PropertyService.Data;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Repositories
{
    public class PropertyImageRepository:Repository<PropertyImage>,IPropertyImageRepository
    {
        protected readonly AppDbContext _db;
        public PropertyImageRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
