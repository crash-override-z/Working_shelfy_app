using Microsoft.AspNetCore.Identity;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb;

public static class DemoCustomerSeeder
{
    private static readonly (string Name, string Email)[] Customers =
    {
        ("Sukhampreet", "sukhampreet@example.com"),
        ("Colin", "colin@example.com"),
        ("Himani", "himani@example.com"),
        ("Eliza", "eliza@example.com"),
        ("Joshua", "joshua@example.com"),
        ("Ariel", "ariel@example.com"),
        ("William", "william@example.com"),
        ("Clara", "clara@example.com"),
        ("Carlos", "carlos@example.com"),
        ("Metok", "metok@example.com"),
        ("Sana", "sana@example.com")
    };

    public static void Seed(ShelfyDbContext db, string password)
    {
        var hasher = new PasswordHasher<CustomerAccount>();

        foreach (var (name, email) in Customers)
        {
            var normalizedEmail = email.Trim().ToUpperInvariant();

            if (db.CustomerAccounts.Any(account => account.Email == normalizedEmail))
            {
                continue;
            }

            var account = new CustomerAccount
            {
                Name = name,
                Email = normalizedEmail
            };

            account.PasswordHash = hasher.HashPassword(account, password);
            db.CustomerAccounts.Add(account);
        }

        db.SaveChanges();
    }
}