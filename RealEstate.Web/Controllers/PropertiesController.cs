using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;
using System.Net.Mime;

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
                model.Property = new PropertyViewModel();
                if (_userService.GetCurrentUser().Role == RoleConstants.Role_User_Comp)
                {
                    var usersResponse = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsers/{_userService.GetCurrentUser().Id}");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                        model.UsersList = users.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    }
                }

                var propertyTypesResponse = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/GetPropertyTypes");
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

                return View("AddUpdateProperty", model);
            }
            else
            {
                var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetPropertyById/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                    model.Property = property;
                    var usersResponse = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsers");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                        model.UsersList = users.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    }
                    var propertyTypesResponse = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/GetPropertyTypes");
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
                    return RedirectToAction("AddProperty", model);
                }
                else
                {
                    return RedirectToAction("AddProperty", model);
                }
            }
        }

        [HttpPost]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> AddProperty(AddPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _userService.GetCurrentUser();
                var addPropertyDto = new AddPropertyDto
                {
                    Property = model.Property,
                    CoverImage = model.CoverImage,
                    PropertyImages = model.PropertyImages,
                    CurrentUserId = currentUser.Id,
                    CurrentUserRole = currentUser.Role
                };
                //var parameters = new
                //{
                //    AddPropertyDto = addPropertyDto,
                //    CurrentUserRole = currentUser.Role,
                //    CurrentUserId = currentUser.Id
                //};

                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/AddProperty", addPropertyDto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid attempt");
                    return RedirectToAction("AddProperty");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid attempt");
                return RedirectToAction("AddProperty");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var properties = new List<PropertyViewModel>();
            var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetProperties/{_userService.GetCurrentUser().Id}/{_userService.GetCurrentUser().Role}");
            if (response.IsSuccessStatusCode)
            {
                properties = await response.Content.ReadFromJsonAsync<List<PropertyViewModel>>();
            }
            return Json(new { data = properties });
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