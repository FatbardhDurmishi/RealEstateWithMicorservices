using RealEstate.Services.PropertyService.Data;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Repositories
{
    public class PropertyTypeRepository:Repository<PropertyType>,IPropertyTypeRepository
    {
        protected readonly AppDbContext _db;

        public PropertyTypeRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
