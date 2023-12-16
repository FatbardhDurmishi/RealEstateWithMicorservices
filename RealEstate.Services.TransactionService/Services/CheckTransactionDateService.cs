
using RealEstate.Services.TransactionService.Constants;
using RealEstate.Services.TransactionService.Repositories.IRepositories;

namespace RealEstate.Services.TransactionService.Services
{
    public class CheckTransactionDateService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public CheckTransactionDateService()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            // Calculate the delay until the next 9 AM
            var now = DateTime.Now;
            var nextNineAM = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);
            if (now > nextNineAM)
            {
                nextNineAM = nextNineAM.AddDays(1);
            }
            var delay = nextNineAM - now;

            // Schedule the task to run every day at 9 AM
            _timer = new Timer(ExecuteTask, null, delay, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        //since the CheckTransactionDateService is as a singleton service we can't inject a scoped service in here like we tried with ITransactionRepository, which caused an error. This is not allowed bc singletond service is created once and lives for the entire lifetime of the application, while scoped service is created once per scope(one per request in a web app).

        // so to resolve this, we should avoid injecting scoped services directly into singleton services. We use the IServiceScopeFactory to create a scope within ur singleton service when u need access to scoped services
        private async void ExecuteTask(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var transactionRepository = scopedServices.GetRequiredService<ITransactionRepository>();

                try
                {
                    var transactions = await transactionRepository.GetAll(x => x.Status == TransactionStatus.Rented);

                    if (transactions == null)
                    {
                        return;
                    }

                    foreach (var transaction in transactions)
                    {
                        if (transaction.RentEndDate < DateTime.UtcNow && transaction.Status != TransactionStatus.Expired)
                        {
                            transaction.Status = TransactionStatus.Expired;

                            var httpClientFactory = scopedServices.GetRequiredService<IHttpClientFactory>();
                            using (var httpClient = httpClientFactory.CreateClient())
                            {
                                var response = await httpClient.GetAsync($"{APIGatewayUrl.URL}/api/property/UpdatePropertyStatus{transaction.PropertyId}/{PropertyStatus.Free}");

                                if (response.IsSuccessStatusCode)
                                {
                                    await transactionRepository.SaveChanges();
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            // Dispose of any resources if needed
            _timer?.Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of any resources if needed
            _timer?.Dispose();
        }
    }
}
