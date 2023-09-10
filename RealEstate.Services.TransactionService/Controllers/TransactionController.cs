using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using RealEstate.Services.TransactionService.Constants;
using RealEstate.Services.TransactionService.Models;
using RealEstate.Services.TransactionService.Models.Dtos;
using RealEstate.Services.TransactionService.Repositories.IRepositories;

namespace RealEstate.Services.TransactionService.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly HttpClient _httpClient;

        public TransactionController(ITransactionRepository transactionRepository, HttpClient httpClient)
        {
            _transactionRepository = transactionRepository;
            _httpClient = httpClient;
        }

        [HttpGet("GetTransactions/{currentUserId}/{currentUserRole}")]
        public async Task<ActionResult> GetTransactions(string currentUserId, string currentUserRole)
        {
            if (currentUserRole == RoleConstants.Role_User_Comp)
            {
                //one option is to get all the users of the company and then get all the transactions of those users
                var response = await _httpClient.GetAsync($"{APIGatewayUrl.URL}api/user/GetUsersByCompanyId/{currentUserId}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    var transactions = await _transactionRepository.GetAll(x => users.Any(y => x.OwnerId == y.Id || x.BuyerId == y.Id));
                    return Ok(transactions);
                }
                else
                {
                    return BadRequest();
                }
            }
            else if (currentUserRole == RoleConstants.Role_User_Indi)
            {
                var transactions = await _transactionRepository.GetAll(x => x.OwnerId == currentUserId || x.BuyerId == currentUserId);
                foreach (var transaction in transactions)
                {
                    if (transaction.OwnerId == currentUserId && transaction.Status != TransactionStatus.Sold && transaction.Status != TransactionStatus.Rented && transaction.Status != TransactionStatus.Denied && transaction.Status != TransactionStatus.Expired)
                    {
                        transaction.ShowButtons = true;
                    }
                }
                return Ok(transactions);
            }
            else
            {
                var transactions = await _transactionRepository.GetAll();
                return Ok(transactions);
            }
        }

        [HttpGet("GetTransaction/{id}")]
        public async Task<ActionResult> GetTransaction(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefault(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost("AddTransaction")]
        public async Task<ActionResult> AddTransaction(AddTransactionDto transactionDto)
        {
            var transaction = _transactionRepository.GetAll(x => x.BuyerId == transactionDto.BuyerId && x.PropertyId == transactionDto.PropertyId).Result.OrderByDescending(x => x.Date).FirstOrDefault();
            if (transaction == null || transaction.Status == TransactionStatus.Denied || transaction.Status == TransactionStatus.Sold)
            {
                var transactionToAdd = new Transaction
                {
                    Status = TransactionStatus.Pending,
                    Date = DateTime.Now,
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

                await _transactionRepository.Add(transactionToAdd);
                return Ok(transactionToAdd);
            }
            else
            {
                if (transaction.TransactionType == TransactionTypes.Rent)
                {
                    return BadRequest("You already requested to rent this property.");
                }
                else
                {
                    return BadRequest("You already requested to buy this property.");
                }
            }
        }

        [HttpPost("ApproveRequest/{id}")]
        public async Task<ActionResult> ApproveRequest(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefault(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            if (transaction.TransactionType == TransactionTypes.Rent)
            {
                _transactionRepository.UpdateStatus(transaction, TransactionStatus.Rented);
                await _transactionRepository.SaveChanges();
                //me shtu ni api call te property service per me ndryshu statusin e property
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
                return Ok();
            }
            else
            {
                var parameters = new
                {
                    propertyId = transaction.PropertyId,
                    status = PropertyStatus.Sold
                };
                _transactionRepository.UpdateStatus(transaction, TransactionStatus.Sold);
                await _transactionRepository.SaveChanges();
                //me shtu ni api call te property service per me ndryshu statusin e property
                var response = await _httpClient.PostAsJsonAsync($"{APIGatewayUrl.URL}api/property/UpdatePropertyStatus", parameters);
                //qetu duhet me ndryshu edhe owner te property

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                return Ok();
            }
        }

        [HttpPost("DenyRequest/{id}")]
        public async Task<ActionResult> DenyRequest(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefault(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            _transactionRepository.UpdateStatus(transaction, TransactionStatus.Denied);
            await _transactionRepository.SaveChanges();
            return Ok();
        }

        [HttpDelete("DeleteTransaction/{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefault(x => x.Id == id);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }
            await _transactionRepository.Remove(transaction);
            return Ok();
        }

        [HttpDelete("DeleteTransactionByPropertyId/{id}")]
        public async Task<ActionResult> DeleteTransactionByPropertyId(int id)
        {
            var transactions = await _transactionRepository.GetAll(x => x.PropertyId == id);
            if (transactions == null)
            {
                return NotFound("Transaction not found");
            }
            foreach (var transaction in transactions)
            {
                await _transactionRepository.Remove(transaction);
            }
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
    }
}