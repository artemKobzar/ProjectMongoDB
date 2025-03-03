using ClientMongo.Pages;
using System.Text.Json;

namespace ClientMongo.Services
{
    public class UserGettingClaimsService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserGettingClaimsService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetName()
        {
            //var isAuthenticated = _httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            var claims = _httpContextAccessor.HttpContext.User?.Claims?.ToList();
            var nameClaim = claims?.FirstOrDefault(c => c.Type == "name")?.Value;
            string userName = "Anonymous";
            if (!string.IsNullOrEmpty(nameClaim))
            {
                try
                {
                    // Deserialize JSON array
                    var nameArray = JsonSerializer.Deserialize<List<string>>(nameClaim);
                    if (nameArray != null && nameArray.Count > 1)
                    {
                        userName = nameArray[1]; // Second value is the name
                    }
                }
                catch
                {
                    // Fallback if JSON parsing fails
                    userName = nameClaim;
                }
            }
            return userName;
        }
    }
}
