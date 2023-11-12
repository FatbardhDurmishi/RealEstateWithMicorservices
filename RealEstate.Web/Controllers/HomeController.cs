using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public HomeController(IUserService userService, HttpClient httpClient)
        {
            _userService = userService;
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userService.GetCurrentUser().Id;
            var userRole = _userService.GetCurrentUser().Role;
            var transactionsList = new List<TransactionListViewModel>();
            if (userRole == RoleConstants.Role_User_Comp)
            {
                var transactionsResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransactions/{_userService.GetCurrentUser().Id}/{_userService.GetCurrentUser().Role}");
                if (transactionsResponse.IsSuccessStatusCode)
                {
                    var transactions = await transactionsResponse.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
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

                            transactionsList.Add(transactionToAdd);
                        }
                    }

                    ViewBag.TodaySale = transactionsList.Where(x => x.Date.Day == DateTime.Today.Day && x.TransactionType == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();
                    //ViewBag.TodaySale = _transactionRepository.GetAll(x => x.Date.Day == DateTime.Today.Day && x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                    ViewBag.TotalSales = transactionsList.Where(x => x.TransactionType == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();
                    //ViewBag.TotalSales = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                    ViewBag.RentThisMonth = transactionsList.Where(x => x.Date.Month >= DateTime.Now.Month && x.Date.Month <= DateTime.Now.Month && x.TransactionType == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();
                    //ViewBag.RentThisMonth = _transactionRepository.GetAll(x => x.RentStartDate.Month >= DateTime.Now.Month && x.RentEndDate.Month <= DateTime.Now.Month && x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();

                    ViewBag.TotaRemt = transactionsList.Where(x => x.TransactionType == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();
                    //ViewBag.TotalRent = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.TotalPrice).Sum();
                    //var latestTransactions = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId || x.Buyer.CompanyId == userId).OrderByDescending(x => x.Date).Take(5);
                    var latestTransactions = transactionsList.OrderByDescending(x => x.Date).Take(5);

                    //ViewBag.Expenses = transactionsList.Where(x =>x.BuyerName.ToLower()== ).Select(x => x.TotalPrice).Sum();
                    //ViewBag.Expenses = _transactionRepository.GetAll(x => x.Buyer.CompanyId == userId).Select(x => x.TotalPrice).Sum();

                    //ViewBag.RentThisYear
                    ViewBag.BestSellThisYear = transactionsList.Where(x => x.Date.Year == DateTime.Now.Year && x.TransactionType == TransactionTypes.Sale).OrderByDescending(x => x.TotalPrice).Select(x => x.TotalPrice).FirstOrDefault();
                    //ViewBag.BestSellThisYear = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.TotalPrice).Select(x => x.TotalPrice).FirstOrDefault();

                    ViewBag.BestRentThisYear = transactionsList.Where(x => x.Date.Year == DateTime.Now.Year && x.TransactionType == TransactionTypes.Rent).OrderByDescending(x => x.RentPrice).Select(x => x.RentPrice).FirstOrDefault();
                    //ViewBag.BestRentThisYear = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.RentPrice).Select(x => x.RentPrice).FirstOrDefault();

                    return View(latestTransactions);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                var transactionsResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/transaction/GetTransactions/{_userService.GetCurrentUser().Id}/{_userService.GetCurrentUser().Role}");
                if (transactionsResponse.IsSuccessStatusCode)
                {
                    var transactions = await transactionsResponse.Content.ReadFromJsonAsync<List<TransactionViewModel>>();
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

                            transactionsList.Add(transactionToAdd);
                        }
                    }

                    ViewBag.TodaySale = transactionsList.Where(x => x.Date.Day == DateTime.Today.Day && x.TransactionType == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();
                    //ViewBag.TodaySale = _transactionRepository.GetAll(x => x.Date.Day == DateTime.Today.Day && x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                    ViewBag.TotalSales = transactionsList.Where(x => x.TransactionType == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();
                    //ViewBag.TotalSales = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                    ViewBag.RentThisMonth = transactionsList.Where(x => x.Date.Month >= DateTime.Now.Month && x.Date.Month <= DateTime.Now.Month && x.TransactionType == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();
                    //ViewBag.RentThisMonth = _transactionRepository.GetAll(x => x.RentStartDate.Month >= DateTime.Now.Month && x.RentEndDate.Month <= DateTime.Now.Month && x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();

                    ViewBag.TotaRemt = transactionsList.Where(x => x.TransactionType == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();
                    //ViewBag.TotalRent = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.TotalPrice).Sum();
                    //var latestTransactions = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId || x.Buyer.CompanyId == userId).OrderByDescending(x => x.Date).Take(5);
                    var latestTransactions = transactionsList.OrderByDescending(x => x.Date).Take(5);

                    ViewBag.Expenses = transactionsList.Where(x => x.BuyerName.ToLower() == _userService.GetCurrentUser().Name.ToLower()).Select(x => x.TotalPrice).Sum();
                    //ViewBag.Expenses = _transactionRepository.GetAll(x => x.Buyer.CompanyId == userId).Select(x => x.TotalPrice).Sum();

                    //ViewBag.RentThisYear
                    ViewBag.BestSellThisYear = transactionsList.Where(x => x.Date.Year == DateTime.Now.Year && x.TransactionType == TransactionTypes.Sale).OrderByDescending(x => x.TotalPrice).Select(x => x.TotalPrice).FirstOrDefault();
                    //ViewBag.BestSellThisYear = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.TotalPrice).Select(x => x.TotalPrice).FirstOrDefault();

                    ViewBag.BestRentThisYear = transactionsList.Where(x => x.Date.Year == DateTime.Now.Year && x.TransactionType == TransactionTypes.Rent).OrderByDescending(x => x.RentPrice).Select(x => x.RentPrice).FirstOrDefault();
                    //ViewBag.BestRentThisYear = _transactionRepository.GetAll(x => x.Owner.CompanyId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.RentPrice).Select(x => x.RentPrice).FirstOrDefault();

                    return View(latestTransactions);
                }
                else
                {
                    return View();
                }
                //ViewBag.TodaySale = _transactionRepository.GetAll(x => x.Date == DateTime.Today && x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                //ViewBag.TotalSales = _transactionRepository.GetAll(x => x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale).Select(x => x.TotalPrice).Sum();

                //ViewBag.RentThisMonth = _transactionRepository.GetAll(x => x.RentStartDate.Month >= DateTime.Now.Month && x.RentEndDate.Month <= DateTime.Now.Month && x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.RentPrice).Sum();

                //ViewBag.TotalRent = _transactionRepository.GetAll(x => x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent).Select(x => x.TotalPrice).Sum();
                ////ViewBag.LatestTransactions = _transactionRepository.GetAll(x => x.OwnerId == userId, includeProperties: "Owner,TransactionTypeNavigation").Take(5).OrderBy(x => x.Date);

                ////var latestTransactions = _transactionRepository.GetAll(x => x.OwnerId == userId || x.BuyerId == userId).Take(5).OrderBy(x => x.Date);
                //ViewBag.Expenses = _transactionRepository.GetAll(x => x.BuyerId == userId).Select(x => x.TotalPrice).Sum();

                ////ViewBag.RentThisYear
                //ViewBag.BestSellThisYear = _transactionRepository.GetAll(x => x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Sale && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.TotalPrice).Select(x => x.TotalPrice).FirstOrDefault();

                //ViewBag.BestRentThisYear = _transactionRepository.GetAll(x => x.OwnerId == userId && x.TransactionTypeNavigation.Name == TransactionTypes.Rent && x.Date.Year == DateTime.Now.Year).OrderByDescending(x => x.RentPrice).Select(x => x.RentPrice).FirstOrDefault();
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
                ViewBag.PropertyTypes = propertyTypes.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            }
            if (loggedIn)
            {
                var userId = _userService.GetCurrentUser().Id;
                var userRole = _userService.GetCurrentUser().Role;

                //ViewBag.PropertyTypes = new SelectList(_propertyTypeRepository.GetAll(), "Id", "Name");
                //ViewBag.Cities = new SelectList(CityConstants._cities, "Name", "Name");
                if (userRole == RoleConstants.Role_User_Comp)
                {
                    var propertiesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex/{userId}/{userRole}");
                    if (propertiesResponse.IsSuccessStatusCode)
                    {
                        var properties = await propertiesResponse.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                        var filtered = properties.AsQueryable();
                        filtered = city != null ? filtered.Where(x => x.City == city) : filtered;
                        filtered = bedrooms.HasValue ? filtered.Where(x => x.BedRooms == bedrooms) : filtered;
                        filtered = bathrooms.HasValue ? filtered.Where(x => x.BathRooms == bathrooms) : filtered;
                        filtered = minPrice.HasValue ? filtered.Where(x => x.Price >= minPrice) : filtered;
                        filtered = maxPrice.HasValue ? filtered.Where(x => x.Price <= maxPrice) : filtered;
                        filtered = propertyType.HasValue ? filtered.Where(x => x.PropertyType.Id == propertyType) : filtered;
                        filtered = transactionType != null ? filtered.Where(x => x.TransactionType == transactionType) : filtered;

                        return View(filtered);
                    }
                }
                else
                {
                    var propertiesResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex/{userId}/{userRole}");
                    if (propertiesResponse.IsSuccessStatusCode)
                    {
                        var properties = await propertiesResponse.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                        var filtered = properties.AsQueryable();
                        filtered = city != null ? filtered.Where(x => x.City == city) : filtered;
                        filtered = bedrooms.HasValue ? filtered.Where(x => x.BedRooms == bedrooms) : filtered;
                        filtered = bathrooms.HasValue ? filtered.Where(x => x.BathRooms == bathrooms) : filtered;
                        filtered = minPrice.HasValue ? filtered.Where(x => x.Price >= minPrice) : filtered;
                        filtered = maxPrice.HasValue ? filtered.Where(x => x.Price <= maxPrice) : filtered;
                        filtered = propertyType.HasValue ? filtered.Where(x => x.PropertyType.Id == propertyType) : filtered;
                        filtered = transactionType != null ? filtered.Where(x => x.TransactionType == transactionType) : filtered;

                        return View(filtered);
                    }
                }
            }

            var allProperties = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/property/GetPropertiesForIndex");
            if (allProperties.IsSuccessStatusCode)
            {
                var properties = await allProperties.Content.ReadFromJsonAsync<IEnumerable<PropertyViewModel>>();
                var filtered = properties.AsQueryable();
                filtered = city != null ? filtered.Where(x => x.City == city) : filtered;
                filtered = bedrooms.HasValue ? filtered.Where(x => x.BedRooms == bedrooms) : filtered;
                filtered = bathrooms.HasValue ? filtered.Where(x => x.BathRooms == bathrooms) : filtered;
                filtered = minPrice.HasValue ? filtered.Where(x => x.Price >= minPrice) : filtered;
                filtered = maxPrice.HasValue ? filtered.Where(x => x.Price <= maxPrice) : filtered;
                filtered = propertyType.HasValue ? filtered.Where(x => x.PropertyType.Id == propertyType) : filtered;
                filtered = transactionType != null ? filtered.Where(x => x.TransactionType == transactionType) : filtered;
                return View(filtered);
            }
            return View();
        }
    }
}