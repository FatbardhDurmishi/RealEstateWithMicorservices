using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.TransactionService.Constants;
using RealEstate.Services.TransactionService.Models;
using RealEstate.Services.TransactionService.Models.Dtos;
using RealEstate.Services.TransactionService.Repositories.IRepositories;
using RealEstate.Services.TransactionService.Services;

namespace RealEstate.Services.TransactionService.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly HttpClient _httpClient;
        private readonly MailService _emailService;

        public TransactionController(ITransactionRepository transactionRepository, HttpClient httpClient, MailService emailService)
        {
            _transactionRepository = transactionRepository;
            _httpClient = httpClient;
            _emailService = emailService;
        }

        [HttpGet("GetTransactions/{currentUserId}/{currentUserRole}")]
        public async Task<ActionResult> GetTransactions(string currentUserId, string currentUserRole)
        {
            var transactionsList = new List<Transaction>();
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                //one option is to get all the users of the company and then get all the transactions of those users
                var usersResponse = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var users = await usersResponse.Content.ReadFromJsonAsync<List<UserDto>>();
                    if (users.Count() > 0)
                    {
                        var transactions = await _transactionRepository.GetAllAsync();
                        transactionsList = transactions?.Where(x => users.Any(y => x.OwnerId == y.Id || x.BuyerId == y.Id)).ToList();
                        _transactionRepository.Dispose();
                        return Ok(transactionsList);
                    }
                    return Ok(transactionsList);

                }
                else
                {
                    return BadRequest();
                }
            }
            else if (currentUserRole == RoleConstants.Role_User_Indi)
            {
                var transactions = await _transactionRepository.GetAllAsync(x => x.OwnerId == currentUserId || x.BuyerId == currentUserId);
                transactionsList = transactions.ToList();
                _transactionRepository.Dispose();
                foreach (var transaction in transactionsList)
                {
                    if (transaction.OwnerId == currentUserId && transaction.Status != TransactionStatus.Sold && transaction.Status != TransactionStatus.Rented && transaction.Status != TransactionStatus.Denied && transaction.Status != TransactionStatus.Expired)
                    {
                        transaction.ShowButtons = true;
                    }
                }
                return Ok(transactionsList);
            }
            else
            {
                var transactions = await _transactionRepository.GetAllAsync();
                transactionsList = transactions.ToList();
                _transactionRepository.Dispose();
                return Ok(transactionsList);
            }
        }

        [HttpGet("GetTransaction/{id}")]
        public async Task<ActionResult> GetTransaction(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            _transactionRepository.Dispose();
            return Ok(transaction);
        }

        [HttpPost("AddTransaction")]
        public async Task<ActionResult> AddTransaction(AddTransactionDto transactionDto)
        {
            var transaction = _transactionRepository.GetAllAsync(x => x.BuyerId == transactionDto.BuyerId && x.PropertyId == transactionDto.PropertyId).Result.OrderByDescending(x => x.Date).FirstOrDefault();
            if (transaction == null || transaction.Status == TransactionStatus.Denied || transaction.Status == TransactionStatus.Sold)
            {
                var transactionToAdd = new Transaction
                {
                    Status = TransactionStatus.Pending,
                    Date = DateTime.UtcNow,
                    OwnerId = transactionDto.OwnerId,
                    BuyerId = transactionDto.BuyerId,
                    PropertyId = transactionDto.PropertyId,
                    TransactionType = transactionDto.TransactionType
                };
                if (transactionDto.TransactionType == TransactionTypes.Rent)
                {
                    transactionToAdd.RentStartDate = transactionDto.RentStartDate;
                    transactionToAdd.RentEndDate = transactionDto.RentEndDate;
                    transactionToAdd.TotalPrice = transactionDto.PropertyPrice * CalculateTotalMonthsForRent(transactionDto.RentStartDate, transactionDto.RentEndDate);
                    transactionToAdd.RentPrice = transactionDto.PropertyPrice;
                }
                else
                {
                    transactionToAdd.TotalPrice = transactionDto.PropertyPrice;
                }

                await _transactionRepository.AddAsync(transactionToAdd);
                await _transactionRepository.SaveChangesAsync();
                _transactionRepository.Dispose();
                var buyer = await GetUser(transactionDto.BuyerId);
                if (buyer != null)
                {
                    var owner = await GetUser(transactionDto.OwnerId);
                    if (owner != null)
                    {
                        await _emailService.SendEmailAsync(owner.Email, "New Request", $"New Reuqest from: {buyer.Email} for property with ID: {transactionToAdd.PropertyId}");
                    }
                }
                return Ok(transactionToAdd);
            }
            else
            {
                if (transaction.TransactionType == TransactionTypes.Rent)
                {
                    return BadRequest(new { Message = "You already requested to rent this property." });
                }
                else
                {
                    return BadRequest(new { Message = "You already requested to buy this property." });
                }
            }
        }

        [HttpPost("ApproveRequest/{id}")]
        public async Task<ActionResult> ApproveRequest(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            var buyer = await GetUser(transaction.BuyerId!);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            if (transaction.TransactionType == TransactionTypes.Rent)
            {
                _transactionRepository.UpdateStatus(transaction, TransactionStatus.Rented);
                await _transactionRepository.SaveChangesAsync();
                _transactionRepository.Dispose();

                var parameters = new
                {
                    propertyId = transaction.PropertyId,
                    status = PropertyStatus.Rented
                };
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/UpdatePropertyStatus", parameters);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                await _emailService.SendEmailAsync(buyer.Email, "Request Approved for Rent", $"Your Rent Request for property with ID: {transaction.PropertyId} was approved");

                return Ok();
            }
            else
            {
                var updatePropertyStatusParameters = new
                {
                    propertyId = transaction.PropertyId,
                    status = PropertyStatus.Sold
                };

                //me shtu ni api call te property service per me ndryshu statusin e property
                var updatePropertyStatusResponse = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/UpdatePropertyStatus", updatePropertyStatusParameters);
                if (!updatePropertyStatusResponse.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                var updatePropertyOwnerParameters = new
                {
                    propertyId = transaction.PropertyId,
                    userId = transaction.BuyerId
                };

                var updatePropertyOwnerResponse = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/UpdatePropertyOwner", updatePropertyOwnerParameters);
                if (!updatePropertyOwnerResponse.IsSuccessStatusCode)
                {
                    return BadRequest();
                }

                _transactionRepository.UpdateStatus(transaction, TransactionStatus.Sold);
                await _transactionRepository.SaveChangesAsync();
                _transactionRepository.Dispose();

                await _emailService.SendEmailAsync(buyer.Email, "Request Approved for Sale", $"Your request to property with ID: {transaction.PropertyId} was approved");
                return Ok();
            }
        }

        [HttpPost("DenyRequest/{id}")]
        public async Task<ActionResult> DenyRequest(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            _transactionRepository.UpdateStatus(transaction, TransactionStatus.Denied);
            await _transactionRepository.SaveChangesAsync();
            _transactionRepository.Dispose();
            var buyer = await GetUser(transaction.BuyerId!);
            if (buyer != null)
            {
                await _emailService.SendEmailAsync(buyer.Email, "Request Denied", $"Your Request for property with ID: {transaction.PropertyId} was Denied");
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteTransaction/{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            _transactionRepository.Remove(transaction);
            await _transactionRepository.SaveChangesAsync();
            _transactionRepository.Dispose();
            return Ok();
        }

        [HttpDelete("DeleteTransactionByPropertyId/{id}")]
        public async Task<ActionResult> DeleteTransactionByPropertyId(int id)
        {
            var transactions = await _transactionRepository.GetAllAsync(x => x.PropertyId == id);
            if (transactions == null)
            {
                return NotFound("Transaction not found");
            }
            foreach (var transaction in transactions)
            {
                _transactionRepository.Remove(transaction);
            }
            await _transactionRepository.SaveChangesAsync();
            _transactionRepository.Dispose();
            return Ok();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private static int CalculateTotalMonthsForRent(DateTime startDate, DateTime endDate)
        {
            int months = 0;
            while (startDate < endDate)
            {
                startDate = startDate.AddMonths(1);
                months++;
            }
            return months;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
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