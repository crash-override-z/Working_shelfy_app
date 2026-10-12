using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb.Pages;

public class IndexModel : PageModel
{
    private readonly ShelfyDbContext _db;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ShelfyDbContext db, ILogger<IndexModel> logger)
    {
        _db = db;
        _logger = logger;
    }

    public static readonly MenuRoll[] Menu =
    {
        new("Fujiya Roll", 10.95m),
        new("Spicy Chopped Tuna Roll", 9.95m),
        new("Spicy Chopped Salmon Roll", 9.95m),
        new("Wild Salmon Maki", 8.50m),
        new("Salmon Maki", 6.25m),
        new("Tuna Maki", 6.25m),
        new("Salmon Avocado Maki", 8.95m),
        new("B.C. Roll", 6.95m),
        new("California Roll", 5.95m),
        new("Shrimp California Roll", 8.95m)
    };

    public static readonly string[] PickupDays =
    {
        "Monday", "Tuesday", "Wednesday", "Thursday", "Friday"
    };

    [BindProperty]
    public string CustomerName { get; set; } = string.Empty;

    [BindProperty]
    public string PickupDay { get; set; } = string.Empty;

    [BindProperty]
    public string PickupTime { get; set; } = string.Empty;

    [BindProperty]
    public List<int> Quantities { get; set; } = new();

    [TempData]
    public string? ConfirmationTicketId { get; set; }

    public int QuantityAt(int index) =>
        index < Quantities.Count ? Quantities[index] : 0;

    public void OnGet()
    {
        Quantities = Enumerable.Repeat(0, Menu.Length).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CustomerName = CustomerName.Trim();

        if (string.IsNullOrWhiteSpace(CustomerName))
        {
            ModelState.AddModelError(nameof(CustomerName), "Enter your name.");
        }
        else if (CustomerName.Length > 100)
        {
            ModelState.AddModelError(nameof(CustomerName), "Name must be 100 characters or fewer.");
        }

        if (!PickupDays.Contains(PickupDay, StringComparer.Ordinal))
        {
            ModelState.AddModelError(nameof(PickupDay), "Choose a valid pickup day.");
        }

        bool validTime = TimeOnly.TryParseExact(
            PickupTime,
            "h:mm tt",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out TimeOnly pickupTime);

        if (!validTime
            || pickupTime < new TimeOnly(12, 0)
            || pickupTime > new TimeOnly(20, 0)
            || pickupTime.Minute % 15 != 0)
        {
            ModelState.AddModelError(nameof(PickupTime), "Choose a pickup time from 12:00 PM to 8:00 PM in 15-minute increments.");
        }

        if (Quantities.Count != Menu.Length || Quantities.Any(quantity => quantity < 0 || quantity > 99))
        {
            ModelState.AddModelError(nameof(Quantities), "Enter a quantity from 0 to 99 for each menu item.");
        }
        else if (!Quantities.Any(quantity => quantity > 0))
        {
            ModelState.AddModelError(nameof(Quantities), "Add at least one item to your order.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        List<OrderItem> items = Menu
            .Select((roll, index) => new { roll, quantity = Quantities[index] })
            .Where(selection => selection.quantity > 0)
            .Select(selection => new OrderItem
            {
                RollName = selection.roll.Name,
                Quantity = selection.quantity,
                UnitPrice = selection.roll.Price
            })
            .ToList();

        decimal subtotal = items.Sum(item => item.UnitPrice * item.Quantity);
        decimal reservationFee = 1.50m;
        decimal tax = decimal.Round(subtotal * 0.05m, 2, MidpointRounding.AwayFromZero);
        decimal total = subtotal + reservationFee + tax;
        string ticketId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        var order = new Order
        {
            TicketId = ticketId,
            CustomerName = CustomerName,
            PickupSlot = $"{PickupDay.ToLowerInvariant()}, {PickupTime}",
            TotalPrice = total,
            IsConfirmed = true,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Items = items
        };

        _db.Orders.Add(order);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Could not save order {TicketId}.", ticketId);
            ModelState.AddModelError(string.Empty, "We could not save your order. Please try again.");
            return Page();
        }

        ConfirmationTicketId = ticketId;
        return RedirectToPage();
    }
}

public sealed record MenuRoll(string Name, decimal Price);