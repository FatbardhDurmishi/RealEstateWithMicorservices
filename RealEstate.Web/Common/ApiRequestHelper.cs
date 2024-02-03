using System.Net.Http.Headers;

namespace RealEstate.Web.Common
{
    public static class ApiRequestHelper
    {
        public static void SetBearerToken(HttpClient httpClient, string? token)
        {

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
