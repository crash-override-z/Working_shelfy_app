using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace ShelfyWeb.Pages
{
    public class AdminModel : PageModel
    {
        public List<string> LiveOrders { get; set; } = new();
        public decimal TotalRevenue { get; set; }

        public void OnGet()
        {
            string csvPath = GetOrdersFilePath();

            if (System.IO.File.Exists(csvPath))
            {
                var lines = System.IO.File.ReadAllLines(csvPath);
                foreach (var line in lines)
                {
                    LiveOrders.Add(line);

                    int lastComma = line.LastIndexOf(',');
                    if (lastComma >= 0
                        && decimal.TryParse(
                            line[(lastComma + 1)..],
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out decimal orderTotal))
                    {
                        TotalRevenue += orderTotal;
                    }
                }
            }
            else
            {
                LiveOrders.Add("Waiting for orders... csv file not found at path.");
            }
        }

        private string GetOrdersFilePath()
        {
            DirectoryInfo? directory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (directory != null)
            {
                if (System.IO.File.Exists(Path.Combine(directory.FullName, "Shelfy.slnx"))
                    || Directory.Exists(Path.Combine(directory.FullName, "ShelfyWeb")))
                {
                    return Path.Combine(directory.FullName, "orders.csv");
                }

                directory = directory.Parent;
            }

            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "orders.csv"));
        }
    }
}