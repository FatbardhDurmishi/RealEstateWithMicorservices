using RealEstate.Web.Constants;
using RealEstate.Web.Services.IServices;

namespace RealEstate.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(Token.TokenCookie);
        }

        public string? GetToken(string userId = "")
        {
            string? token = null;
            bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(userId == null ? "" : userId, out token);

            return hasToken is true ? token : null;
        }

        public void SetToken(string userId, string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(userId, token);
        }
    }
}
