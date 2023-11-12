using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RealEstate.Web.Controllers
{
    [AllowAnonymous]
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
                    var claims = DecodeToken(loginResponse.Token);

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    if (loginResponse.User.Role == RoleConstants.Role_Admin)
                    {
                        TempData["success"] = "Login successful";
                        return RedirectToAction("Index", "User");
                    }
                    TempData["success"] = "Login successful";
                    return RedirectToAction("Dashboard", "Home");
                }
            }
            //else
            //{
            //    ModelState.AddModelError("", "Invalid login attempt");
            //    TempData["error"] = "Login not successful";
            //    return View(model);
            //}
            ModelState.AddModelError("", "Invalid login attempt");
            TempData["error"] = "Login not successful";
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
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid register attempt");
                return View(model);
            }
            var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/auth/register", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Login));
            }

            ModelState.AddModelError("", "Invalid register attempt");
            return View(model);

        }

        [HttpPost()]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            _userService.RemoveCurrentUser();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private List<Claim> DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? result = handler.ReadToken(token) as JwtSecurityToken;

            if (result == null)
            {
                return new List<Claim>();
            }
            return result.Claims.ToList();
        }
    }
}