
using RealEstate.Services.PropertyService.Models;

namespace RealEstate.Services.PropertyService.Repositories.IRepositories
{
    public interface IPropertyRepository:IRepository<Property>
    {
        string UpdateStatus(Property property, string status);
    }
}
