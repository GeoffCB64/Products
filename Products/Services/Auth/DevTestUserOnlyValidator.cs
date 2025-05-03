namespace Products.Services.Auth
{
    /// <summary>
    /// Dev/test only. This class uses hardcoded credentials.  Do not use in production.
    /// </summary>
    public class DevTestUserOnlyValidator : IUserValidator
    {
        private readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase)
    {
        { "username1", "pass123" },
        { "username2", "letmein" },
        { "admin", "adminpass" }
    };

        /// <summary>
        ///  Dev/test only. This method uses a dictionary with hardcoded credentials.  Do not use in production.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            return _users.TryGetValue(username, out var expectedPassword)
                && expectedPassword == password;
        }
    }
}
