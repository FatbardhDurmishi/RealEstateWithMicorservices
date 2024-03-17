using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstate.Web.Common;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Role_User_Indi + "," + RoleConstants.Role_User_Comp)]
    public class PropertiesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly ITokenProvider _tokenProvider;

        public PropertiesController(HttpClient httpClient, IUserService userService, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _userService = userService;
            _tokenProvider = tokenProvider;
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken(_userService.GetCurrentUser().Id));
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
                        model.UsersList = users!.Select(x => new SelectListItem { Text = x.Name, Value = x.Id });
                    }
                }
                var propertyTypesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
                if (propertyTypesResponse.IsSuccessStatusCode)
                {
                    var propertyTypes = await propertyTypesResponse.Content.ReadFromJsonAsync<List<PropertyType>>();
                    model.PropertyTypeList = propertyTypes!.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
                model.Cities = CityConstants._cities.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                });
                return View("AddUpdateProperty", model);
            }
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{id}");
            if (response.IsSuccessStatusCode)
            {
                var property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                model.Property = property!;
                if (currentUserRole == RoleConstants.Role_User_Comp)
                {
                    var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsers/{currentUserId}");
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                        model.UsersList = users!.Select(x => new SelectListItem { Text = x.Name, Value = x.Id });
                    }
                }
                var propertyTypesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
                if (propertyTypesResponse.IsSuccessStatusCode)
                {
                    var propertyTypes = await propertyTypesResponse.Content.ReadFromJsonAsync<List<PropertyType>>();
                    model.PropertyTypeList = propertyTypes!.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                }
                model.Cities = CityConstants._cities.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                });
                return View("AddUpdateProperty", model);
            }
            return View("AddUpdateProperty");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProperty(AddPropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid input. Please try again.";
                ModelState.AddModelError("Errors", "Invalid attempt");
                return RedirectToAction("AddProperty", model);
            }

            var currentUser = _userService.GetCurrentUser();
            var propertyDto = MapToPropertyDto(model, currentUser);

            var propertyCreationResponse = await CreateProperty(propertyDto, model.CoverImage, model.PropertyImages);

            if (propertyCreationResponse.IsSuccessStatusCode)
            {
                TempData["success"] = "Property added successfully";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Failed to add property. Please try again.";
            return RedirectToAction("AddProperty", model);
        }

        private async Task<HttpResponseMessage> CreateProperty(PropertyDto propertyDto, IFormFile coverImage, IFormFileCollection propertyImages)
        {
            var propertyCreationResponse = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/AddProperty", propertyDto);
            if (propertyCreationResponse.IsSuccessStatusCode)
            {
                var property = await propertyCreationResponse.Content.ReadFromJsonAsync<PropertyDto>();
                var imageUploadResponse = await UploadImages(property!.Id, coverImage, propertyImages);
                if (imageUploadResponse.IsSuccessStatusCode)
                {
                    return propertyCreationResponse;
                }
                await DeleteProperty(property.Id);
            }
            return propertyCreationResponse;
        }

        private PropertyDto MapToPropertyDto(AddPropertyViewModel model, UserDto currentUser)
        {
            return new PropertyDto
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
                CurrentUserRole = currentUser.Role!,
            };
        }

        private async Task<HttpResponseMessage> UploadImages(int propertyId, IFormFile coverImage, IFormFileCollection
         propertyImages)
        {
            var content = new MultipartFormDataContent { { new StreamContent(coverImage.OpenReadStream()), "CoverImage", coverImage.FileName } };
            foreach (var image in propertyImages)
            {
                content.Add(new StreamContent(image.OpenReadStream()), "PropertyImages", image.FileName);
            }
            return await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/property/UploadImages/{propertyId}", content);
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var currentUserId = _userService.GetCurrentUser().Id;
            var currentUserRole = _userService.GetCurrentUser().Role;
            var properties = new List<PropertyViewModel>();
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperties/{currentUserId}/{currentUserRole}");
            if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                properties = await response.Content.ReadFromJsonAsync<List<PropertyViewModel>>();

                foreach (var property in properties!)
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
                            Name = user!.Name
                        };
                    }
                }
                return new JsonResult(properties);
            }
            return new JsonResult(properties);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/property/DeleteProperty/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Delete Not Successful" });
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

                var user = await GetUser(propertyDetailsViewModel.Property!.UserId!);
                if (user != null)
                {
                    propertyDetailsViewModel.User = user;
                    return View(propertyDetailsViewModel);
                }
                TempData["error"] = "Something went wrong when trying to read user data! Try again";
                if (!HttpContext.User!.Identity!.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProperty(AddPropertyViewModel model, int[]? DeleteImageArr)
        {
            ModelState.Remove("DeleteImageArr");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid attempt");
                return View("AddUpdateProperty", model);
            }
            var updatePropertyDto = new PropertyDto
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
                CoverImageUrl = model.CoverImage?.FileName,
                TransactionType = model.Property.TransactionType,
                UserId = model.Property.UserId,
                PropertyTypeId = model.Property.PropertyTypeId
            };

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
                    var uploadImagesResponse = await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/property/UploadImages/{property!.Id}", content);
                    if (!uploadImagesResponse.IsSuccessStatusCode)
                    {
                        TempData["error"] = "Something went wrong while uploading new images! Try again";
                        return View("Index");
                    }
                }
                TempData["success"] = "Property updated successfully";
                return View("Index");
            }
            ModelState.AddModelError("", "Invalid attempt");
            return View("AddUpdateProperty", model);

        }


        public async Task<UserDto> GetUser(string userId)
        {
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return user!;
            }
            return null!;
        }

    }
}
