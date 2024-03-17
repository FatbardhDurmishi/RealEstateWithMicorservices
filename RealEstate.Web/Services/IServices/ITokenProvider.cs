namespace RealEstate.Web.Services.IServices
{
    public interface ITokenProvider
    {
        void SetToken(string userId, string token);
        string? GetToken(string userId);
        void ClearToken(string userId);
    }
}
