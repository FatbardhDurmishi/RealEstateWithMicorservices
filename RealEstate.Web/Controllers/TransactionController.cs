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
        public async Task<IActionResult> AddTransaction(int propertyId)
        {
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{propertyId}");
            if (response.IsSuccessStatusCode)
            {
                var property = await response.Content.ReadFromJsonAsync<PropertyViewModel>();
                AddTranscationViewModel transaction = new()
                {
                    Property = property,
                    Transaction = new()
                };
                return View(transaction);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(AddTranscationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var transactionDto = new AddTransactionDto()
                {
                    PropertyId = model.Property.Id,
                    BuyerId = _userService.GetCurrentUser().Id,
                    OwnerId = model.Property.UserId,
                    RentStartDate = model.Transaction.RentStartDate,
                    RentEndDate = model.Transaction.RentEndDate,
                    TransactionType = model.Transaction.TransactionType,
                    PropertyPrice = model.Property.Price
                };
                //var parameters = new
                //{
                //    property = model.Property,
                //    transaction = model.Transaction,
                //    userId = _userService.GetCurrentUser().Id
                //};
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/transaction/AddTransaction", transactionDto);
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
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransactions/{currentUserId}/{currentUserRole}");
            if (response.IsSuccessStatusCode)
            {
                var transactiosList = new List<TransactionListViewModel>();
                var transactions = await response.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
                foreach (var transaction in transactions)
                {
                    var transactionToAdd = new TransactionListViewModel()
                    {
                        Id = transaction.Id,
                        TransactionType = transaction.TransactionType,
                        Status = transaction.Status,
                        TotalPrice = transaction.TotalPrice,
                        RentPrice = transaction.RentPrice,
                    };

                    var buyer = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{transaction.BuyerId}");
                    if (buyer.IsSuccessStatusCode)
                    {
                        var buyerUser = await buyer.Content.ReadFromJsonAsync<UserDto>();
                        transactionToAdd.BuyerName = buyerUser.Name;
                    }
                    var owner = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{transaction.OwnerId}");
                    if (owner.IsSuccessStatusCode)
                    {
                        var ownerUser = await owner.Content.ReadFromJsonAsync<UserDto>();
                        transactionToAdd.OwnerName = ownerUser.Name;
                    }
                    var property = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{transaction.PropertyId}");
                    if (property.IsSuccessStatusCode)
                    {
                        var propertyDetails = await property.Content.ReadFromJsonAsync<PropertyViewModel>();
                        transactionToAdd.PropertyName = propertyDetails.Name;
                        //if (transaction.Status == TransactionStatus.Rented)
                        //{
                        //    transactionToAdd.RentPrice = transaction.RentPrice;
                        //}
                        //else
                        //{
                        //    transactionToAdd.TotalPrice = transaction.TotalPrice;
                        //}
                    }
                    if (transaction.OwnerId == currentUserId && transaction.Status != TransactionStatus.Sold && transaction.Status != TransactionStatus.Rented && transaction.Status != TransactionStatus.Denied && transaction.Status != TransactionStatus.Expired)
                    {
                        transactionToAdd.ShowButtons = true;
                    }

                    transactiosList.Add(transactionToAdd);
                }
                return new JsonResult(transactiosList);
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
            var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransaction/{id}");
            if (response.IsSuccessStatusCode)
            {
                var transaction = await response.Content.ReadFromJsonAsync<TransactionViewModel>();
                transactionDetails.Transaction = transaction;
                var property = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{transaction.PropertyId}");
                if (property.IsSuccessStatusCode)
                {
                    var propertyDetails = await property.Content.ReadFromJsonAsync<PropertyViewModel>();
                    transactionDetails.Property = propertyDetails;
                }
                var buyer = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{transaction.BuyerId}");
                if (buyer.IsSuccessStatusCode)
                {
                    var buyerDetails = await buyer.Content.ReadFromJsonAsync<UserDto>();
                    transactionDetails.Buyer = buyerDetails;
                }
                var owner = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUser/{transaction.OwnerId}");
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

        public async Task<IActionResult> ApproveRequest(int id)
        {
            var response = await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/transaction/ApproveRequest/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(nameof(Index));
            }
        }

        public async Task<IActionResult> RejectRequest(int id)
        {
            var response = await _httpClient.PostAsync($"{APIGatewayUrl.URL}api/transaction/DenyRequest/{id}", null);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{APIGatewayUrl.URL}api/transaction/DeleteTransaction/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(nameof(Index));
            }
        }
    }
}