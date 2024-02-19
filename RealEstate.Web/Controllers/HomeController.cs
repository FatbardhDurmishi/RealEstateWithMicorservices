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
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;

        public HomeController(IUserService userService, HttpClient httpClient, ITokenProvider tokenProvider)
        {
            _userService = userService;
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken());
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.Role_User_Indi + "," + RoleConstants.Role_User_Comp + ",")]
        public async Task<IActionResult> Dashboard()
        {
            ApiRequestHelper.SetBearerToken(_httpClient, _tokenProvider.GetToken());
            var userId = _userService.GetCurrentUser().Id;
            var userRole = _userService.GetCurrentUser().Role;
            var transactionsList = new List<TransactionListViewModel>();
            var transactions = new List<TransactionViewModel>();
            if (userRole == RoleConstants.Role_User_Comp)
            {
                var transactionsResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransactions/{userId}/{userRole}");
                var users = new List<UserDto>();
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{userId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();

                }
                if (transactionsResponse.IsSuccessStatusCode)
                {
                    transactions = await transactionsResponse.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
                    if (transactions != null && transactions.Count > 0)
                    {
                        foreach (var transaction in transactions)
                        {
                            var transactionToAdd = new TransactionListViewModel()
                            {
                                Id = transaction.Id,
                                TransactionType = transaction.TransactionType,
                                Status = transaction.Status!,
                                TotalPrice = transaction.TotalPrice,
                                RentPrice = transaction.RentPrice,
                            };

                            var buyer = await GetUser(transaction.BuyerId!);
                            if (buyer != null)
                            {
                                transactionToAdd.BuyerName = buyer.Name;
                            }
                            var owner = await GetUser(transaction.OwnerId!);
                            if (owner != null)
                            {
                                transactionToAdd.OwnerName = owner.Name;
                            }
                            var property = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{transaction.PropertyId}");
                            if (property.IsSuccessStatusCode)
                            {
                                var propertyDetails = await property.Content.ReadFromJsonAsync<PropertyViewModel>();
                                transactionToAdd.PropertyName = propertyDetails!.Name;

                                transactionsList.Add(transactionToAdd);
                            }
                        }
                    }

                    ViewBag.TodaySale = transactions?.Where(x => x.Date.Day == DateTime.Today.Day
                    && x.TransactionType == TransactionTypes.Sale
                    && users!.Any(y => x.OwnerId == y.Id))
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.TotalSales = transactions?.Where(x => x.TransactionType == TransactionTypes.Sale &&
                    x.Status == TransactionStatus.Sold
                    && users!.Any(y => x.OwnerId == y.Id))
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.RentThisMonth = transactions?.Where(x => x.RentStartDate.Month >= DateTime.Now.Month
                    && x.RentEndDate.Month <= DateTime.Now.Month
                    && x.TransactionType == TransactionTypes.Rent
                    && users!.Any(y => x.OwnerId == y.Id))
                        .Select(x => x.RentPrice)
                        .Sum();

                    ViewBag.TotalRent = transactions?.Where(x => x.TransactionType == TransactionTypes.Rent
                    && users!.Any(y => x.OwnerId == y.Id))
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.Expenses = transactions?.Where(x => users!.Any(y => x.BuyerId == y.Id)
                    && x.Status != TransactionStatus.Pending)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.BestSellThisYear = transactions?.Where(x => x.Date.Year == DateTime.Now.Year
                        && x.TransactionType == TransactionTypes.Sale
                        && x.Status == TransactionStatus.Sold
                        && users!.Any(y => x.OwnerId == y.Id))
                        .OrderByDescending(x => x.TotalPrice)
                        .Select(x => x.TotalPrice)
                        .FirstOrDefault();

                    ViewBag.BestRentThisYear = transactions?.Where(x => x.Date.Year == DateTime.Now.Year
                        && x.TransactionType == TransactionTypes.Rent
                        && x.Status == TransactionStatus.Rented
                        && users!.Any(y => x.OwnerId == y.Id))
                        .OrderByDescending(x => x.RentPrice)
                        .Select(x => x.RentPrice)
                        .FirstOrDefault();

                    ViewBag.TotalProfit = transactions?.Where(x => users!.Any(y => x.OwnerId == y.Id))
                        .Select(x => x.TotalPrice)
                        .Sum();

                    var latestTransactions = transactionsList?.OrderByDescending(x => x.Date).Take(5);


                    return View(latestTransactions);
                }
                else
                {
                    TempData["error"] = "Something went wrong while retrieving data";
                    return View();
                }
            }
            else
            {
                var transactionsResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransactions/{userId}/{userRole}");
                if (transactionsResponse.IsSuccessStatusCode)
                {
                    transactions = await transactionsResponse.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
                    if (transactions != null && transactions.Any())
                    {
                        foreach (var transaction in transactions)
                        {
                            var transactionToAdd = new TransactionListViewModel()
                            {
                                Id = transaction.Id,
                                TransactionType = transaction.TransactionType,
                                Status = transaction.Status!,
                                TotalPrice = transaction.TotalPrice,
                                RentPrice = transaction.RentPrice,
                            };

                            var buyer = await GetUser(transaction.BuyerId!);
                            if (buyer != null)
                            {
                                transactionToAdd.BuyerName = buyer.Name;
                            }
                            var owner = await GetUser(transaction.OwnerId!);
                            if (owner != null)
                            {
                                transactionToAdd.OwnerName = owner.Name;
                            }
                            var property = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetProperty/{transaction.PropertyId}");
                            if (property.IsSuccessStatusCode)
                            {
                                var propertyDetails = await property.Content.ReadFromJsonAsync<PropertyViewModel>();
                                transactionToAdd.PropertyName = propertyDetails!.Name;

                                transactionsList.Add(transactionToAdd);
                            }
                        }
                    }

                    ViewBag.TodaySale = transactions?.Where(x => x.Date.Day == DateTime.Today.Day
                        && x.TransactionType == TransactionTypes.Sale
                        && x.Status == TransactionStatus.Sold
                        && x.OwnerId == userId)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.TotalSales = transactions?.Where(x => x.TransactionType == TransactionTypes.Sale
                        && x.Status == TransactionStatus.Sold
                        && x.OwnerId == userId)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.RentThisMonth = transactions?.Where(x => x.RentStartDate.Month >= DateTime.Now.Month
                        && x.RentEndDate.Month <= DateTime.Now.Month
                        && x.TransactionType == TransactionTypes.Rent
                        && x.Status == TransactionStatus.Rented && x.OwnerId == userId)
                        .Select(x => x.RentPrice)
                        .Sum();

                    ViewBag.TotalRent = transactions?.Where(x => x.TransactionType == TransactionTypes.Rent
                        && x.Status == TransactionStatus.Rented
                        && x.OwnerId == userId)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.Expenses = transactions?.Where(x => x.BuyerId == userId
                        && x.Status != TransactionStatus.Pending)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    ViewBag.BestSellThisYear = transactions?.Where(x => x.Date.Year == DateTime.Now.Year
                        && x.TransactionType == TransactionTypes.Sale
                        && x.Status == TransactionStatus.Sold
                        && x.OwnerId == userId)
                        .OrderByDescending(x => x.TotalPrice)
                        .Select(x => x.TotalPrice)
                        .FirstOrDefault();

                    ViewBag.BestRentThisYear = transactions?.Where(x => x.Date.Year == DateTime.Now.Year
                        && x.TransactionType == TransactionTypes.Rent
                        && x.Status == TransactionStatus.Rented
                        && x.OwnerId == userId)
                        .OrderByDescending(x => x.RentPrice)
                        .Select(x => x.RentPrice)
                        .FirstOrDefault();

                    ViewBag.TotalProfit = transactions?.Where(x => x.OwnerId == userId)
                        .Select(x => x.TotalPrice)
                        .Sum();

                    var latestTransactions = transactionsList?.OrderByDescending(x => x.Date).Take(5);

                    return View(latestTransactions);
                }
                else
                {
                    TempData["error"] = "Something went wrong while retrieving data";
                    return View();
                }
            }
        }

        //I guess this works having two http verbs in the same method lol
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Index(string? city, int? bedrooms, int? bathrooms, decimal? minPrice, decimal? maxPrice, int? propertyType, string? transactionType, bool loggedIn)
        {
            var propertyTypesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/propertyTypes/GetPropertyTypes");
            if (propertyTypesResponse.IsSuccessStatusCode)
            {
                var propertyTypes = await propertyTypesResponse.Content.ReadFromJsonAsync<List<PropertyType>>();
                ViewBag.PropertyTypes = propertyTypes?.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }
            if (loggedIn)
            {
                var userId = _userService.GetCurrentUser().Id;
                var userRole = _userService.GetCurrentUser().Role;

                if (userRole == RoleConstants.Role_User_Comp)
                {
                    var propertiesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex/{userId}/{userRole}");
                    if (propertiesResponse.IsSuccessStatusCode)
                    {
                        var properties = await propertiesResponse.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                        properties = city != null ? properties?.Where(x => x.City == city) : properties;
                        properties = bedrooms.HasValue ? properties?.Where(x => x.BedRooms == bedrooms) : properties;
                        properties = bathrooms.HasValue ? properties?.Where(x => x.BathRooms == bathrooms) : properties;
                        properties = minPrice.HasValue ? properties?.Where(x => x.Price >= minPrice) : properties;
                        properties = maxPrice.HasValue ? properties?.Where(x => x.Price <= maxPrice) : properties;
                        properties = propertyType.HasValue ? properties?.Where(x => x.PropertyType?.Id == propertyType) : properties;
                        properties = transactionType != null ? properties?.Where(x => x.TransactionType == transactionType) : properties;

                        return View(properties);
                    }
                }
                else
                {
                    var propertiesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex/{userId}/{userRole}");
                    if (propertiesResponse.IsSuccessStatusCode)
                    {
                        var properties = await propertiesResponse.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                        properties = city != null ? properties?.Where(x => x.City == city) : properties;
                        properties = bedrooms.HasValue ? properties?.Where(x => x.BedRooms == bedrooms) : properties;
                        properties = bathrooms.HasValue ? properties?.Where(x => x.BathRooms == bathrooms) : properties;
                        properties = minPrice.HasValue ? properties?.Where(x => x.Price >= minPrice) : properties;
                        properties = maxPrice.HasValue ? properties?.Where(x => x.Price <= maxPrice) : properties;
                        properties = propertyType.HasValue ? properties?.Where(x => x.PropertyType?.Id == propertyType) : properties;
                        properties = transactionType != null ? properties?.Where(x => x.TransactionType == transactionType) : properties;

                        return View(properties);
                    }
                }
            }

            var allProperties = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex");
            if (allProperties.IsSuccessStatusCode)
            {
                var properties = await allProperties.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                properties = city != null ? properties?.Where(x => x.City == city) : properties;
                properties = bedrooms.HasValue ? properties?.Where(x => x.BedRooms == bedrooms) : properties;
                properties = bathrooms.HasValue ? properties?.Where(x => x.BathRooms == bathrooms) : properties;
                properties = minPrice.HasValue ? properties?.Where(x => x.Price >= minPrice) : properties;
                properties = maxPrice.HasValue ? properties?.Where(x => x.Price <= maxPrice) : properties;
                properties = propertyType.HasValue ? properties?.Where(x => x.PropertyType?.Id == propertyType) : properties;
                properties = transactionType != null ? properties?.Where(x => x.TransactionType == transactionType) : properties;
                return View(properties);
            }
            return View();
        }

        private async Task<UserDto> GetUser(string userId)
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