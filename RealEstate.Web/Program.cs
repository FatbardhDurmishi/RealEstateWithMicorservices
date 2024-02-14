using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Web.Constants;
using RealEstate.Web.Services;
using RealEstate.Web.Services.IServices;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
//builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogout = context =>
        {
            // Set the expiration time to 1 second, making it a session cookie
            context.Options.ExpireTimeSpan = TimeSpan.FromSeconds(1);
            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.HttpContext.Response.Cookies.Delete(Token.TokenCookie);
            return Task.CompletedTask;
        },
    };
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Secret"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["ApiSettings:Audience"],
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();


builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();

var app = builder.Build();

var cookiePolicyOptions = new CookiePolicyOptions
{
    CheckConsentNeeded = context => false,
    MinimumSameSitePolicy = SameSiteMode.None,
};

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy(cookiePolicyOptions);


app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
        );

app.Run();
