using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public PropertiesController(HttpClient httpClient, IUserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddProperty(int? id)
        {
            var model = new AddPropertyViewModel();
            if (id == null)
            {
                var usersResponse = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsers");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    model.UsersList = users.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
                var propertyTypesResponse = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetPropertyTypes");
                if (propertyTypesResponse.IsSuccessStatusCode)
                {
                    var propertyTypes = await propertyTypesResponse.Content.ReadFromJsonAsync<List<PropertyType>>();
                    model.PropertyTypeList = propertyTypes.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
                model.Cities = CityConstants._cities.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                });

                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty(AddPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var addPropertyDto = new AddPropertyDto
                {
                    Property = model.Property,
                    CoverImage = model.CoverImage,
                    PropertyImages = model.PropertyImages
                };
                var currentUser = _userService.GetCurrentUser();
                var parameters = new
                {
                    AddPropertyDto = addPropertyDto,
                    CurrentUserRole = currentUser.Role,
                    CurrentUserId = currentUser.Id
                };
                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/AddProperty", parameters);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetProperties/{_userService.GetCurrentUser().Id}/{_userService.GetCurrentUser().Role}");
            if (response.IsSuccessStatusCode)
            {
                var properties = await response.Content.ReadFromJsonAsync<List<PropertyViewModel>>();
                return Json(new { data = properties });
            }
            else
            {
                return View();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/DeleteProperty/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            else
            {
                return Json(new { success = false, message = "Delete Not Successful" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int propertyId)
        {
            PropertyDetailsViewModel propertyDetailsViewModel = new PropertyDetailsViewModel();
            var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetPropertyById/{propertyId}");
            if (response.IsSuccessStatusCode)
            {
                propertyDetailsViewModel.Property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                var userResponse = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUserById/{propertyDetailsViewModel.Property.UserId}");
                if (userResponse.IsSuccessStatusCode)
                {
                    propertyDetailsViewModel.User = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                }

                return View(propertyDetailsViewModel);
            }
            else
            {
                return View();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProperty(AddPropertyViewModel model, int[] DeleteImageIdArr)
        {
            if (ModelState.IsValid)
            {
                var updatePropertyDto = new AddPropertyDto
                {
                    Property = model.Property,
                    CoverImage = model.CoverImage,
                    PropertyImages = model.PropertyImages
                };
                var parameters = new
                {
                    AddPropertyDto = updatePropertyDto,
                    ImagesToDelete = DeleteImageIdArr
                };
                var response = await _httpClient.PutAsJsonAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/UpdateProperty", parameters);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid attempt");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid attempt");
                return View(model);
            }
        }
    }
}