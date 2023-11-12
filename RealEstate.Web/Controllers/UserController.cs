using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.CustomAttributes;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;

namespace RealEstate.Web.Controllers
{
    [AuthorizeUsers(RoleConstants.Role_Admin, RoleConstants.Role_User_Comp)]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public UserController(HttpClient httpClient, IUserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(string? id)
        {
            if (id == null)
            {
                RegisterViewModel model = new RegisterViewModel();
                return View("CreateUpdateUser", model);
            }
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<RegisterViewModel>();
                var model = new RegisterViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    StreetAddres = user.StreetAddres,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode
                };

                return View("CreateUpdateUser", model);
            }
            else
            {
                return View("CreateUpdateUser");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserRole = _userService.GetCurrentUser().Role;
                var currentUserId = _userService.GetCurrentUser().Id;

                CreateUserDto user = new CreateUserDto()
                {
                    User = model,
                    CurrentUserId = currentUserId,
                    CurrentUserRole = currentUserRole
                };


                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/user/CreateUser", user);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View("CreateUpdateUser", model);
            }
            return View("CreateUpdateUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(RegisterViewModel model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/user/UpdateUser", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid attempt");
                return View("CreateUpdateUser", model);
            }
            return View("CreateUpdateUser", model);
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetUsersJson()
        {
            var currentUserRole = _userService.GetCurrentUser().Role;
            var currentUserId = _userService.GetCurrentUser().Id;

            if (currentUserRole == RoleConstants.Role_Admin)
            {
                var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}/{currentUserRole}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return new JsonResult(users);
                }
            }
            else
            {
                var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return new JsonResult(users);
                }
            }
            return new JsonResult(null);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var response = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/user/DeleteUser/{id}");
            if (response.IsSuccessStatusCode)
            {
                return new JsonResult(new { message = "Delete Successful", success = true });
            }
            return new JsonResult(new { message = "Delete Not Successful", success = false });
        }

        #endregion API CALLS
    }
}