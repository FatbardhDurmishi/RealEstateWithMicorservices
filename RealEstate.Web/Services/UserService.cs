using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;
using System.Runtime.CompilerServices;

namespace RealEstate.Web.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {
        }

        private UserDto? CurrentUser
        {
            get; set;
        }

        public UserDto GetCurrentUser()
        {
            if (CurrentUser == null)
            {
                return null!;
            }
            return CurrentUser;
        }

        public string GetCurrentUserRole()
        {
            if (CurrentUser == null)
            {
                return " ";
            }
            return CurrentUser.Role!;
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