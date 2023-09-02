using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.TransactionService.Constants;
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

        [HttpGet]
        public async Task<ActionResult> GetTransactions(string currentUserId,string currentUserRole)
        {

            //if user is company
            if (currentUserRole == "Company")
            {
                //one option is to get all the users of the company and then get all the transactions of those users
                var response = await _httpClient.GetAsync($"{APIBaseUrls.AuthAPIBaseUrl}api/user/GetUsersByCompanyId/{currentUserId}");
                if(response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                    var transactions = await _transactionRepository.GetAll(x=>users.Any(y=>x.OwnerId==y.Id));
                    return Ok(transactions);


                }
                else
                {
                    return BadRequest();
                }
            }
            else if(currentUserRole == "User")
            {
                var transactions = await  _transactionRepository.GetAll(x => x.OwnerId == currentUserId || x.BuyerId == currentUserId);
                foreach(var transaction in transactions)
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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetTransaction(int id)
        {
            var transaction = await _transactionRepository.GetFirstOrDefault(x=>x.Id==id, includeProperties: "TransactionType");
            if(transaction==null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }
    }
}
