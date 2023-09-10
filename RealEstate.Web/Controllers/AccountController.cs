using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public AccountController(HttpClient httpClient, IUserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/auth/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    _userService.SetCurrentUser(loginResponse.User);

                    if (loginResponse.User.Role == RoleConstants.Role_Admin)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/auth/Register", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid register attempt");
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _userService.RemoveCurrentUser();
            return RedirectToAction(nameof(Login));
        }
    }
}