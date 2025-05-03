namespace Products.Services.Auth
{
    public interface IUserValidator
    {
        bool ValidateCredentials(string username, string password);
    }
}
