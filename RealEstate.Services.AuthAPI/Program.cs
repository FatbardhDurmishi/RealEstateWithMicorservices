using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Services.AuthAPI.Data;
using RealEstate.Services.AuthAPI.Models;
using RealEstate.Services.AuthAPI.Repositories;
using RealEstate.Services.AuthAPI.Repositories.IRepository;
using RealEstate.Services.AuthAPI.Service.IService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//var mySqlConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");

//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();

builder.Services.AddControllers();

var secret = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Secret");
var issuer = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Issuer");
var audience = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Audience");

var key = Encoding.ASCII.GetBytes(secret!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience
    };
});
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//ApplyMigrations();

app.Run();

//void ApplyMigrations()
//{
//    using var serviceScope = app.Services.CreateScope();
//    var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
//    if (context!.Database.GetPendingMigrations().Count() > 0)
//    {
//        context.Database.Migrate();
//    }
//}
