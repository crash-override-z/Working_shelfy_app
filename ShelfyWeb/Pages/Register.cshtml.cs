using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb.Pages;

public class RegisterModel : PageModel
{
    private readonly ShelfyDbContext _db;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(ShelfyDbContext db, ILogger<RegisterModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    [BindProperty]
    public RegistrationInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public class RegistrationInput
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.Name = Input.Name?.Trim() ?? string.Empty;
        Input.Email = Input.Email?.Trim() ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        string normalizedEmail = Input.Email.ToUpperInvariant();

        if (await _db.CustomerAccounts.AnyAsync(
                account => account.Email == normalizedEmail))
        {
            ModelState.AddModelError(
                "Input.Email",
                "An account with that email already exists.");
            return Page();
        }

        var account = new CustomerAccount
        {
            Name = Input.Name,
            Email = normalizedEmail
        };

        account.PasswordHash =
            new PasswordHasher<CustomerAccount>()
                .HashPassword(account, Input.Password);

        _db.CustomerAccounts.Add(account);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Could not create a customer account.");
            ErrorMessage = "We could not create your account. Please try again.";
            return Page();
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Name),
            new Claim(ClaimTypes.Email, Input.Email),
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

        return RedirectToPage("/Index");
    }
}