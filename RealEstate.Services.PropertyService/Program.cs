using Microsoft.EntityFrameworkCore;
using RealEstate.Services.PropertyService.Data;
using RealEstate.Services.PropertyService.Repositories;
using RealEstate.Services.PropertyService.Repositories.IRepositories;
using Newtonsoft.Json;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient();

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).
    AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.AllowInputFormatterExceptionMessages = false;
    }
);

builder.Services.AddTransient<IPropertyRepository, PropertyRepository>();
builder.Services.AddTransient<IPropertyTypeRepository, PropertyTypeRepository>();
builder.Services.AddTransient<IPropertyImageRepository, PropertyImageRepository>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["ConnectionStrings:AzureStorageConnection:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["ConnectionStrings:AzureStorageConnection:queue"], preferMsi: true);
});

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