using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shelfy.Data;
using Shelfy.Models;

namespace ShelfyWeb.Pages
{
    public class AdminModel : PageModel
    {
        private readonly ShelfyDbContext _db;

        public AdminModel(ShelfyDbContext db)
        {
            _db = db;
        }
        

        public List<Order> Orders { get; private set; } = new();

        

        public decimal TotalRevenue => Orders
            .Where(order => order.Status != "Cancelled")
            .Sum(order => order.TotalPrice);

        public static readonly string[] StatusOptions =
        {
            "Pending",
            "Preparing",
            "Ready",
            "Completed",
            "Cancelled"
        };

        public async Task OnGetAsync()
        {
            await LoadOrdersAsync();
        }

        


        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            if (!StatusOptions.Contains(status, StringComparer.Ordinal))
            {
                return BadRequest();
            }

            var order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

       private async Task LoadOrdersAsync()
{
    Orders = await _db.Orders
        .Include(order => order.Items)
        .OrderByDescending(order => order.CreatedAt)
        .ThenByDescending(order => order.Id)
        .ToListAsync();
        }
    }
}