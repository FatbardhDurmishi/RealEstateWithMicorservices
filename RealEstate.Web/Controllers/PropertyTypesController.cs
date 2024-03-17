using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Common;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Role_Admin)]
    public class PropertyTypesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly IUserService _userService;

        public PropertyTypesController(HttpClient httpClient, ITokenProvider tokenProvider, IUserService userService)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _userService = userService;
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken(_userService.GetCurrentUser().Id));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddPropertyType(int? id)
        {
            if (id == null)
            {
                var model = new PropertyType();
                return View("AddUpdatePropertyType", model);
            }
            var propertyTypeResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyType/{id}");
            if (propertyTypeResponse.IsSuccessStatusCode)
            {
                var propertyType = await propertyTypeResponse.Content.ReadFromJsonAsync<PropertyType>();
                return View("AddUpdatePropertyType", propertyType);
            }
            TempData["error"] = "Something went wrong while reading property type! Try again";
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPropertyType(PropertyType propertyType)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all fields";
                ModelState.AddModelError("", "Invalid property type");
                return View("AddUpdatePropertyType", propertyType);
            }
            var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/propertyTypes/AddPropertyType", propertyType);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Property Type added succesfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Something went wrong! Try again";
            ModelState.AddModelError("", "Something went wrong");
            return View("AddUpdatePropertyType", propertyType);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePropertyType(PropertyType propertyType)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fill all the fields";
                ModelState.AddModelError("", "Invalid property type");
                return View("AddUpdatePropertyType", propertyType);
            }
            var response = await _httpClient.PutAsJsonAsync($"{APIGatewayUrl.URL}api/propertyTypes/UpdatePropertyType", propertyType);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Property type updated succesfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Something went wrong! Try again";
            ModelState.AddModelError("", "Something went wrong");
            return View("AddUpdatePropertyType", propertyType);

        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyType/{id}");
            if (response.IsSuccessStatusCode)
            {
                var propertyType = await response.Content.ReadFromJsonAsync<PropertyType>();
                return View(propertyType);
            }
            TempData["error"] = "Something went wrong while reading property type data! Try again";
            return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/propertyTypes/DeletePropertyType/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Property type deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Something went wrong! Try again";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetPropertyTypes()
        {
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
            if (response.IsSuccessStatusCode)
            {
                var propertyTypes = await response.Content.ReadFromJsonAsync<List<PropertyType>>();
                return new JsonResult(propertyTypes);
            }
            return Json(null);
        }
    }
}