﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Common;
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
        private readonly ITokenProvider _tokenProvider;

        public AccountController(HttpClient httpClient, IUserService userService, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _userService = userService;
            _tokenProvider = tokenProvider;
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/auth/login", model);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    _userService.SetCurrentUser(loginResponse!.User);
                    _tokenProvider.SetToken(loginResponse!.Token);

                    await SignInUser(loginResponse);

                    TempData["success"] = "Login successful";
                    if (loginResponse.User.Role == RoleConstants.Role_Admin)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            TempData["error"] = "Login not successful";
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Register(string? userId)
        {
            if (userId == null)
            {
                RegisterViewModel model = new RegisterViewModel();
                return View(model);
            }
            var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var user = await userResponse.Content.ReadFromJsonAsync<RegisterViewModel>();
                return View("Update", user);
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ModelState.Remove("OldPassword");
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

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenProvider.ClearToken();
            _userService.RemoveCurrentUser();
            return RedirectToAction(nameof(Login));
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(RegisterViewModel model)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("OldPassword");
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/auth/updateUserData", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Successfully updated your info";
                    return View("Update", model);
                }
            }
            TempData["error"] = "Something went wrong! Try again";
            return View("Register", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{userId}");
            if (userResponse.IsSuccessStatusCode)
            {
                var user = await userResponse.Content.ReadFromJsonAsync<RegisterViewModel>();
                return View("ChangePassword", user);
            }
            TempData["error"] = "Something went wrong! Try again";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(RegisterViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/auth/changePassword", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Successfully updated your info";
                return RedirectToAction("Dashboard", "Home");
            }
            TempData["error"] = "Something went wrong! Try again";
            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Email, jwt.Claims.FirstOrDefault(U => U.Type == JwtRegisteredClaimNames.Email)!.Value));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(U => U.Type == JwtRegisteredClaimNames.Sub)!.Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(U => U.Type == JwtRegisteredClaimNames.Name)!.Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(U => U.Type == "role")!.Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}