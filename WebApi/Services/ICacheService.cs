namespace WebApi.Services
{
    public interface ICacheService
    {
        void SetCodeForConfirmationEmail(string key, string code);
        string GetCodeForConfirmationEmail(string key);
        void DeleteFromCache(string key);
    }
}