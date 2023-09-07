using RealEstate.Services.PropertyService.Data;
using RealEstate.Services.PropertyService.Models;
using RealEstate.Services.PropertyService.Repositories.IRepositories;

namespace RealEstate.Services.PropertyService.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        protected readonly AppDbContext _db;

        public PropertyRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public string UpdateStatus(Property property, string status)
        {
            //property.Status = status;
            return status;
        }
    }
}