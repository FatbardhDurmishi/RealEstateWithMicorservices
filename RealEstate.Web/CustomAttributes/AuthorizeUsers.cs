using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RealEstate.Web.CustomAttributes
{


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeUsers : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeUsers(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // this checks if a AllowAnonymous is before a method
            if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any()) return;
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new RedirectResult("/Account/Login"); // Redirect to login page if not authenticated
                return;
            }

            // Check if the user has any of the required roles
            if (!_roles.Any(role => user.HasClaim(c => c.Type == "role" && c.Value == role)))
            {
                context.Result = new RedirectResult("/Account/AccessDenied"); // User doesn't have the required role
                return;
            }
        }
    }

}
