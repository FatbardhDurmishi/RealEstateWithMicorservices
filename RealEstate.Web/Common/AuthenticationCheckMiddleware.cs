using Microsoft.AspNetCore.Authentication;
using RealEstate.Web.Constants;

namespace RealEstate.Web.Common
{
    public class AuthenticationCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var result = await context.AuthenticateAsync();

            if (!context.Response.HasStarted) // Check if a response has already started to avoid infinite loops
            {
                if (result.Succeeded)
                {
                    return;
                }
                else
                {
                    if (!context.Request.Path.StartsWithSegments("/Account/Login"))
                    {
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                    if (!context.Request.Path.StartsWithSegments("/Home/Index"))
                    {
                        context.Response.Redirect("/Home/Index");
                        return;
                    }
                    if (!context.Request.Path.StartsWithSegments("/Account/Register"))
                    {
                        context.Response.Redirect("/Account/Register");
                        return;
                    }
                }
            }

            // Continue processing the request pipeline
            await _next(context);
        }
    }
}
