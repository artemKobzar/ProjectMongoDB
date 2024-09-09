using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClientMongo.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration config;

        public LogoutModel(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = config["applicationUrl"]
                },
                "oidc",
                "Cookies");
        }
    }
}
