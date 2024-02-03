using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Common;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Role_User_Comp + "," + RoleConstants.Role_Admin)]
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly ITokenProvider _tokenProvider;

        public UserController(HttpClient httpClient, IUserService userService, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _userService = userService;
            _tokenProvider = tokenProvider;
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken());
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

                return View("CreateUpdateUser", user);
            }
            TempData["error"] = "Something went wrong when reading user! Try again";
            return View("CreateUpdateUser");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            ModelState.Remove("OldPassword");
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all the fields";
                return View("CreateUpdateUser", model);
            }
            var currentUserRole = _userService.GetCurrentUser().Role;
            var currentUserId = _userService.GetCurrentUser().Id;

            CreateUserDto user = new CreateUserDto()
            {
                User = model,
                CurrentUserId = currentUserId,
                CurrentUserRole = currentUserRole!
            };


            var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/user/CreateUser", user);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return View("CreateUpdateUser", model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser(RegisterViewModel model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("OldPassword");
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all the fields";
                return View("CreateUpdateUser", model);
            }
            var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/user/UpdateUser", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "User updated succesfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Something went wrong! Try again";
            return View("CreateUpdateUser", model);
        }




        [HttpGet]
        public async Task<IActionResult> GetUsersJson()
        {
            var currentUserRole = _userService.GetCurrentUser().Role;
            var currentUserId = _userService.GetCurrentUser().Id;
            var users = new List<UserDto>();

            if (currentUserRole == RoleConstants.Role_Admin)
            {
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}/{currentUserRole}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    return new JsonResult(users);
                }
            }
            var companyUsersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
            if (companyUsersResponse.IsSuccessStatusCode)
            {
                users = await companyUsersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                return new JsonResult(users);
            }
            return new JsonResult(users);
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


    }
}