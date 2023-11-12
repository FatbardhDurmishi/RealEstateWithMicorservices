using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using RealEstate.Web.Services;
using RealEstate.Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminPolicy", policy =>
//    {
//        policy.RequireClaim(ClaimTypes.Role, RoleConstants.Role_Admin);
//    });
//    options.AddPolicy("CompanyOrAdminPolicy", policy =>
//    {
//        policy.RequireAssertion(context =>
//        {
//            return context.User.HasClaim(c =>
//                (c.Type == ClaimTypes.Role && c.Value == RoleConstants.Role_User_Indi) ||
//                (c.Type == ClaimTypes.Role && c.Value == RoleConstants.Role_Admin));
//        });
//    });

//    options.AddPolicy("CompanyOrIndividualPolicy", policy =>
//    {
//        policy.RequireAssertion(context =>
//        {
//            return context.User.HasClaim(c =>
//                (c.Type == ClaimTypes.Role && c.Value == RoleConstants.Role_User_Indi) ||
//                (c.Type == ClaimTypes.Role && c.Value == RoleConstants.Role_User_Comp));
//        });
//    });

//    options.AddPolicy("CompanyPolicy", policy =>
//    {
//        policy.RequireClaim(ClaimTypes.Role,RoleConstants.Role_User_Comp);
//    });
//});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
    options.Limits.MaxRequestBufferSize = long.MaxValue;
    options.Limits.MaxResponseBufferSize = long.MaxValue;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient("httpClient", options =>
{
    options.MaxResponseContentBufferSize = long.MaxValue;
});

var app = builder.Build();

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


app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "deafultArea",
        pattern: "{controller}/{action}/{id?}"
        );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}"
        );
});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();