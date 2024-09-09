using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel;
using DuendeIdentity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Azure.Core;
using DuendeIdentity.Data;

namespace DuendeIdentity.Pages.Account.Registration
{
    public class IndexRegistrationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexRegistrationModel(
            UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleInManager
              )
        {
            _roleManager = roleInManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public RegistrationRequest Input { get; set; }

        public async Task<IActionResult> OnGet(string returnUrl)
        {
            List<string> roles = new()
            {
                "Admin",
                "User",
                "Participant"
            };
            ViewData["roles_message"] = roles;
            Input = new RegistrationRequest
            {
                ReturnUrl = returnUrl
            };
            return Page();
        }

        public async Task<IActionResult> OnPost(string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = Input.UserName,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
                if (existingEmail == null)
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");

                        await _userManager.AddClaimsAsync(user, new Claim[] 
                        {
                            new Claim(JwtClaimTypes.Name,Input.Email),
                            new Claim(JwtClaimTypes.Email,Input.Email)
                        });

                        var loginresult = await _signInManager.PasswordSignInAsync(
                            Input.Email, Input.Password, false, lockoutOnFailure: true);

                        if (loginresult.Succeeded)
                        {
                            if (Url.IsLocalUrl(Input.ReturnUrl))
                            {
                                return Redirect(Input.ReturnUrl);
                            }
                            if (string.IsNullOrEmpty(Input.ReturnUrl))
                            {
                                return Redirect("~/");
                            }
                            else
                            {
                                throw new Exception("invalid return URL");
                            }
                        }
                    }
                }
            }
            return Page();
        }
    }
}
