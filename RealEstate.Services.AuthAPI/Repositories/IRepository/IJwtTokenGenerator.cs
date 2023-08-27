using RealEstate.Services.AuthAPI.Models;

namespace RealEstate.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
