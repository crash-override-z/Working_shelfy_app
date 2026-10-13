using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShelfyWeb.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public LoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string? adminPassword = _configuration["AdminPassword"];

            if (string.IsNullOrEmpty(adminPassword))
            {
                ErrorMessage = "The admin password has not been configured.";
                return Page();
            }

            if (Password != adminPassword)
            {
                ErrorMessage = "That password is incorrect.";
                return Page();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToPage("/Admin");
        }
    }
}