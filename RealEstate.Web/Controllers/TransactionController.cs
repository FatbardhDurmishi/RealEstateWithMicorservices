using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Web.Constants;
using RealEstate.Web.Models;
using RealEstate.Web.Models.Dtos;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public TransactionController(HttpClient httpClient, IUserService userService)
        {
            _httpClient = httpClient;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddTranscation(PropertyViewModel property)
        {
            AddTranscationViewModel transaction = new()
            {
                Property = property,
                Transaction = new()
            };
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleConstants.Role_User_Indi)]
        public async Task<IActionResult> AddTranscation(AddTranscationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parameters = new
                {
                    TransactionDto = model,
                    userId = _userService.GetCurrentUser().Id
                };
                var response = await _httpClient.PostAsJsonAsync($"{APIBaseUrls.TransactionAPIBaseUrl}api/transaction", parameters);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var currentUserId = _userService.GetCurrentUser().Id;
            var currentUserRole = _userService.GetCurrentUser().Role;
            var response = await _httpClient.GetAsync($"{APIBaseUrls.TransactionAPIBaseUrl}api/transaction/{currentUserId}/{currentUserRole}");
            if (response.IsSuccessStatusCode)
            {
                var transactions = await response.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
                return Json(new { data = transactions });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            TransactionDetailsViewModel transactionDetails = new();
            var response = await _httpClient.GetAsync($"{APIBaseUrls.TransactionAPIBaseUrl}api/transaction/GetTransaction/{id}");
            if (response.IsSuccessStatusCode)
            {
                var transaction = await response.Content.ReadFromJsonAsync<TransactionViewModel>();
                transactionDetails.Transaction = transaction;
                var property = await _httpClient.GetAsync($"{APIBaseUrls.PropertyAPIBaseUrl}api/property/GetProperty/{transaction.PropertyId}");
                if (property.IsSuccessStatusCode)
                {
                    var propertyDetails = await property.Content.ReadFromJsonAsync<PropertyViewModel>();
                    transactionDetails.Property = propertyDetails;
                }
                var buyer = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUser/{transaction.BuyerId}");
                if (buyer.IsSuccessStatusCode)
                {
                    var buyerDetails = await buyer.Content.ReadFromJsonAsync<UserDto>();
                    transactionDetails.Buyer = buyerDetails;
                }
                var owner = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUser/{transaction.OwnerId}");
                if (owner.IsSuccessStatusCode)
                {
                    var ownerDetails = await owner.Content.ReadFromJsonAsync<UserDto>();
                    transactionDetails.Owner = ownerDetails;
                }

                return View(transactionDetails);
            }
            else
            {
                return View();
            }
        }
    }
}