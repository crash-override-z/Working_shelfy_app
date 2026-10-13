using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb.Pages;

public class LookupModel : PageModel
{
    private readonly ShelfyDbContext _db;

    public LookupModel(ShelfyDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string TicketId { get; set; } = string.Empty;

    public Order? FoundOrder { get; private set; }

    public bool HasSearched { get; private set; }

    public async Task OnGetAsync()
    {
        TicketId = TicketId?.Trim().ToUpperInvariant() ?? string.Empty;

        if (TicketId.Length == 0)
        {
            return;
        }

        HasSearched = true;

        if (TicketId.Length != 8)
        {
            ModelState.AddModelError(nameof(TicketId), "Enter the 8-character confirmation number.");
            return;
        }

        FoundOrder = await _db.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.TicketId == TicketId);
    }
}