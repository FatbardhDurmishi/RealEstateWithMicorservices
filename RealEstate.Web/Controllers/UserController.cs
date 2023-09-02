using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using System.IdentityModel.Tokens.Jwt;

namespace RealEstate.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string token = Request.Cookies["token"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

                var currentUserRole = jwtSecurityToken.Claims.First(x => x.Type == "Role").Value;
                var currentUserId = jwtSecurityToken.Claims.First(x => x.Type == "Sub").Value;

                var parameters = new
                {
                    User = model,
                    CurrentUserRole = currentUserRole,
                    CurrentUserId = currentUserId
                };


                var response = await _httpClient.PostAsJsonAsync("api/user/CreateUser", parameters);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            return View(model);
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetUsersJson()
        {
            string token = Request.Cookies["token"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            var currentUserRole = jwtSecurityToken.Claims.First(x => x.Type == "Role").Value;
            var currentUserId = jwtSecurityToken.Claims.First(x => x.Type == "Sub").Value;

            if(currentUserRole == RoleConstants.Role_Admin)
            {
                var response = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsers");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return Json(new { data = users });
                }
            }
            else
            {
                var response = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsers/{currentUserId}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    return Json(new { data = users });
                }
            }
            return View();

        }

        #endregion
    }
}
