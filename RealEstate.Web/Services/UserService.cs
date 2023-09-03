using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {
        }

        public UserDto? CurrentUser { get; set; }

        public UserDto GetCurrentUser()
        {
            return CurrentUser;
        }

        public void RemoveCurrentUser()
        {
            CurrentUser = null;
        }

        public void SetCurrentUser(UserDto user)
        {
            CurrentUser = user;
        }
    }
}