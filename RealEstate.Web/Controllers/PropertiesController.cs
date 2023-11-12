using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Web.Constants;
using RealEstate.Web.CustomAttributes;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    [AuthorizeUsers(RoleConstants.Role_User_Indi, RoleConstants.Role_User_Comp)]
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
            string currentUserId = _userService.GetCurrentUser().Id;
            string currentUserRole = _userService.GetCurrentUserRole();
            var model = new AddPropertyViewModel();
            if (id == null)
            {
                model.Property = new PropertyViewModel();
                if (currentUserRole == RoleConstants.Role_User_Comp)
                {
                    var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                        model.UsersList = users.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    }
                }

                var propertyTypesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
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
                var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                    model.Property = property;
                    if (currentUserRole == RoleConstants.Role_User_Comp)
                    {
                        var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}");
                        if (usersResponse.IsSuccessStatusCode)
                        {
                            var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                            model.UsersList = users.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                        }
                    }
                    var propertyTypesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
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
                    return View("AddUpdateProperty");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProperty(AddPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _userService.GetCurrentUser();
                var propertyDto = new PropertyDto()
                {
                    Id = model.Property.Id,
                    Name = model.Property.Name,
                    Description = model.Property.Description,
                    BathRooms = model.Property.BathRooms,
                    BedRooms = model.Property.BedRooms,
                    Area = model.Property.Area,
                    Price = model.Property.Price,
                    Status = model.Property.Status,
                    State = model.Property.State,
                    City = model.Property.City,
                    StreetAddress = model.Property.StreetAddress,
                    CoverImageUrl = model.CoverImage.FileName,
                    TransactionType = model.Property.TransactionType,
                    UserId = model.Property.UserId,
                    PropertyTypeId = model.Property.PropertyTypeId,
                    CurrentUserId = currentUser.Id,
                    CurrentUserRole = currentUser.Role,
                };

                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/AddProperty", propertyDto);
                if (response.IsSuccessStatusCode)
                {
                    var property = await response.Content.ReadFromJsonAsync<PropertyDto>();
                    var content = new MultipartFormDataContent
                    {
                        { new StreamContent(model.CoverImage.OpenReadStream()), "CoverImage", model.CoverImage.FileName }
                    };
                    foreach (var image in model.PropertyImages)
                    {
                        content.Add(new StreamContent(image.OpenReadStream()), "PropertyImages", image.FileName);
                    }
                    var uploadImagesResponse = await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/property/UploadImages/{property.Id}", content);
                    if (uploadImagesResponse.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "Something went wrong! Try Again";
                        return View("AddUpdateProperty", model);
                    }
                }
                else
                {
                    TempData["error"] = "Something went wrong! Try Again";
                    ModelState.AddModelError("", "Invalid attempt");
                    return RedirectToAction("AddProperty", model);
                }
            }
            else
            {
                TempData["error"] = "Something went wrong! Try Again";
                ModelState.AddModelError("Errors", "Invalid attempt");
                return View("AddUpdateProperty", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperties/{_userService.GetCurrentUser().Id}/{_userService.GetCurrentUser().Role}");
            if (response.IsSuccessStatusCode)
            {
                var properties = await response.Content.ReadFromJsonAsync<List<PropertyViewModel>>();
                //get the user name for each property
                foreach (var property in properties)
                {
                    if (property.Status != PropertyStatus.Rented)
                    {
                        property.ShowButtons = true;
                    }
                    var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{property.UserId}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var user = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                        property.User = new UserDto()
                        {
                            Name = user.Name
                        };
                    }
                }
                return new JsonResult(properties);
            }
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/property/DeleteProperty/{id}");
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
        [AllowAnonymous]
        public async Task<IActionResult> Details(int propertyId)
        {
            PropertyDetailsViewModel propertyDetailsViewModel = new PropertyDetailsViewModel();
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{propertyId}");
            if (response.IsSuccessStatusCode)
            {
                propertyDetailsViewModel.Property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                var userResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{propertyDetailsViewModel.Property.UserId}");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProperty(AddPropertyViewModel model, int[]? DeleteImageArr)
        {
            ModelState.Remove("DeleteImageArr");
            if (ModelState.IsValid)
            {
                var updatePropertyDto = new PropertyDto();

                updatePropertyDto.Id = model.Property.Id;
                updatePropertyDto.Name = model.Property.Name;
                updatePropertyDto.Description = model.Property.Description;
                updatePropertyDto.BathRooms = model.Property.BathRooms;
                updatePropertyDto.BedRooms = model.Property.BedRooms;
                updatePropertyDto.Area = model.Property.Area;
                updatePropertyDto.Price = model.Property.Price;
                updatePropertyDto.Status = model.Property.Status;
                updatePropertyDto.State = model.Property.State;
                updatePropertyDto.City = model.Property.City;
                updatePropertyDto.StreetAddress = model.Property.StreetAddress;
                if (model.CoverImage != null)
                {
                    updatePropertyDto.CoverImageUrl = model.CoverImage.FileName;
                }
                updatePropertyDto.TransactionType = model.Property.TransactionType;
                updatePropertyDto.UserId = model.Property.UserId;
                updatePropertyDto.PropertyTypeId = model.Property.PropertyTypeId;

                var parameters = new
                {
                    addPropertyDto = updatePropertyDto,
                    imagesToDelete = DeleteImageArr
                };
                var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/property/UpdateProperty", parameters);
                if (response.IsSuccessStatusCode)
                {
                    var property = await response.Content.ReadFromJsonAsync<PropertyDto>();
                    var content = new MultipartFormDataContent();
                    if (model.CoverImage != null)
                    {
                        content.Add(new StreamContent(model.CoverImage.OpenReadStream()), "CoverImage", model.CoverImage.FileName);
                    }
                    if (model.PropertyImages != null)
                    {
                        foreach (var image in model.PropertyImages)
                        {
                            content.Add(new StreamContent(image.OpenReadStream()), "PropertyImages", image.FileName);
                        }
                    }
                    if (content.Any())
                    {
                        var uploadImagesResponse = await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/property/UploadImages/{property.Id}", content);
                        if (uploadImagesResponse.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    return View("AddUpdateProperty", model);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid attempt");
                    return View("AddUpdateProperty", model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid attempt");
                return View("AddUpdateProperty", model);
            }
        }
    }
}