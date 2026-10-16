using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb.Pages;

[Authorize(Roles = "Customer")]
public class MyOrdersModel : PageModel
{
    private readonly ShelfyDbContext _db;

    public MyOrdersModel(ShelfyDbContext db)
    {
        _db = db;
    }

    public List<Order> Orders { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int customerAccountId))
        {
            return Forbid();
        }

        Orders = await _db.Orders
            .AsNoTracking()
            .Where(order => order.CustomerAccountId == customerAccountId)
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .ToListAsync();

        return Page();
    }
}
