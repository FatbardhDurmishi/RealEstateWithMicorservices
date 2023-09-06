using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;

namespace RealEstate.Web.Controllers
{
    public class PropertyTypesController : Controller
    {
        private readonly HttpClient _httpClient;

        public PropertyTypesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            var propertyTypeResponse = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/GetPropertyType/{id}");
            if (propertyTypeResponse.IsSuccessStatusCode)
            {
                var propertyType = await propertyTypeResponse.Content.ReadFromJsonAsync<PropertyType>();
                return View("AddUpdatePropertyType", propertyType);
            }
            return View("AddUpdatePropertyType");
        }

        [HttpPost]
        public async Task<IActionResult> AddPropertyType(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/AddPropertyType", propertyType);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View("AddUpdatePropertyType", propertyType);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid property type");
                return View("AddUpdatePropertyType", propertyType);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePropertyType(PropertyType propertyType)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/UpdatePropertyType", propertyType);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View("AddUpdatePropertyType", propertyType);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid property type");
                return View("AddUpdatePropertyType", propertyType);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/GetPropertyType/{id}");
            if (response.IsSuccessStatusCode)
            {
                var propertyType = await response.Content.ReadFromJsonAsync<PropertyType>();
                return View(propertyType);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/DeletePropertyType/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> GetPropertyTypes()
        {
            var response = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/propertyTypes/GetPropertyTypes");
            if (response.IsSuccessStatusCode)
            {
                var propertyTypes = await response.Content.ReadFromJsonAsync<List<PropertyType>>();
                return new JsonResult(propertyTypes);
            }
            return Json(null);
        }
    }
}