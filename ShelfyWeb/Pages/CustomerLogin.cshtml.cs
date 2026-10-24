using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;

namespace ShelfyWeb.Pages;

public class CustomerLoginModel : PageModel
{
    private readonly ShelfyDbContext _db;

    public CustomerLoginModel(ShelfyDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class LoginInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Email = Input.Email?.Trim() ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        string normalizedEmail = Input.Email.ToUpperInvariant();

        var account = await _db.CustomerAccounts
            .SingleOrDefaultAsync(customer => customer.Email == normalizedEmail);

        if (account is null)
        {
            ErrorMessage = "The email or password is incorrect.";
            return Page();
        }

        var passwordHasher = new PasswordHasher<Shelfy.Models.CustomerAccount>();
        var passwordResult = passwordHasher.VerifyHashedPassword(
            account,
            account.PasswordHash,
            Input.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "The email or password is incorrect.";
            return Page();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Name),
            new Claim(ClaimTypes.Email, account.Email),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            "CookieAuth",
            principal,
            new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            });

        return RedirectToPage("/Home");
    }
}