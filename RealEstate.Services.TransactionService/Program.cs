using Microsoft.EntityFrameworkCore;
using RealEstate.Services.TransactionService.Controllers;
using RealEstate.Services.TransactionService.Data;
using RealEstate.Services.TransactionService.Repositories;
using RealEstate.Services.TransactionService.Repositories.IRepositories;
using RealEstate.Services.TransactionService.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient();


builder.Services.AddControllers();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
builder.Services.AddTransient<EmailService>();
//AddHostedService is automatically as singleton service
builder.Services.AddHostedService<CheckTransactionDateService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

