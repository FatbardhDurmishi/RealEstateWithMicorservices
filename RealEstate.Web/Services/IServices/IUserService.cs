using RealEstate.Web.Models.Dtos;

namespace RealEstate.Web.Services.IServices
{
    public interface IUserService
    {
        void SetCurrentUser(UserDto user);

        UserDto GetCurrentUser();

        void RemoveCurrentUser();

        string GetCurrentUserRole();
    }
}