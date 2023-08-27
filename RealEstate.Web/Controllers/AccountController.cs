using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;

namespace RealEstate.Web.Controllers
{
    public class AccountController : Controller
    {

        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/auth/Login", model);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    Response.Cookies.Append("token", loginResponse.Token);

                    return RedirectToAction("Index", "Home");
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
        public IActionResult Register(RegisterViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/auth/Register", model);
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
            Response.Cookies.Delete("token");
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
