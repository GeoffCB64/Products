using System.Net.Http.Json;
using System.Text;


namespace Products.Tests.Helpers
{
    public static class AuthorisationHelper
    {
        public static async Task<string> GetJwtTokenAsync(HttpClient client, string username = "username1", string password = "pass123")
        {
            var response = await client.PostAsJsonAsync("/login", new { username, password });
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return json!["token"];
        }
    }
}
