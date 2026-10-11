using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ShelfyWeb.Pages
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Clear the authentication cookie
            await HttpContext.SignOutAsync("CookieAuth");

            // Redirect back to the home page
            return RedirectToPage("/Index");
        }
    }
}