using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Shelfy.Data;
using Shelfy.Models;
using Microsoft.EntityFrameworkCore;

namespace ShelfyConsole
{
    class Program
    {
        class RollOption
        {
            public string RollName { get; }
            public decimal Price { get; }

            public RollOption(string name, decimal price)
            {
                RollName = name;
                Price = price;
            }
        }

        static readonly List<RollOption> AvailableRolls = new()
        {
            new RollOption("Fujiya Roll", 10.95m),
            new RollOption("Spicy Chopped Tuna Roll", 9.95m),
            new RollOption("Spicy Chopped Salmon Roll", 9.95m),
            new RollOption("Wild Salmon Maki", 8.50m),
            new RollOption("Salmon Maki", 6.25m),
            new RollOption("Tuna Maki", 6.25m),
            new RollOption("Salmon Avocado Maki", 8.95m),
            new RollOption("B.C. Roll", 6.95m),
            new RollOption("California Roll", 5.95m),
            new RollOption("Shrimp California Roll", 8.95m)
        };

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("         SHELFY ORDER SYSTEM      ");
                Console.WriteLine("==================================");
                Console.Write("Enter customer name (or type admin): ");
                string customerName = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.Equals(customerName, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    ShowAdminDashboard();
                    return;
                }

                if (string.IsNullOrWhiteSpace(customerName))
                {
                    customerName = "Guest";
                }

                List<(RollOption roll, int quantity)> cart = new();
                if (!BuildCart(cart))
                {
                    Console.WriteLine("\nNo order was placed.");
                    if (!AskToOrderAgain())
                    {
                        return;
                    }
                    continue;
                }

                string pickupDay = GetPickupDay();
                string pickupTime = GetPickupTime();

                decimal subtotal = cart.Sum(item => item.roll.Price * item.quantity);
                decimal reservationFee = 1.50m;
                decimal tax = subtotal * 0.05m;
                decimal total = subtotal + reservationFee + tax;
                string ticketId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("          ORDER PREVIEW           ");
                Console.WriteLine("==================================");
                Console.WriteLine($"Customer: {customerName}");
                Console.WriteLine("Items:");
                foreach (var item in cart)
                {
                    Console.WriteLine($"  {item.quantity}x {item.roll.RollName} @ ${item.roll.Price:F2}");
                }
                Console.WriteLine($"Pickup: {pickupDay.ToUpperInvariant()} @ {pickupTime}");
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Subtotal:        ${subtotal:F2}");
                Console.WriteLine($"Reservation Fee: ${reservationFee:F2}");
                Console.WriteLine($"Tax:             ${tax:F2}");
                Console.WriteLine($"TOTAL:           ${total:F2}");

                if (!AskYesNo("\nConfirm and place this order? (y/n): "))
                {
                    Console.WriteLine("\nOrder cancelled.");
                    if (!AskToOrderAgain())
                    {
                        return;
                    }
                    continue;
                }

    // Save the order to the database. ///////////////////////////////////////////////
              try
{
    var options = new DbContextOptionsBuilder<ShelfyDbContext>()
        .UseSqlite($"Data Source={ShelfyDbContext.GetDatabasePath()}")
        .Options;

    using var db = new ShelfyDbContext(options);
    db.Database.EnsureCreated();

    db.Orders.Add(new Order
    {
        TicketId = ticketId,
        CustomerName = customerName,
        PickupSlot = $"{pickupDay.ToLowerInvariant()}, {pickupTime}",
        TotalPrice = total,
        IsConfirmed = true,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    });

    db.SaveChanges();
}
catch (Exception ex)
{
    Console.WriteLine($"\nCould not save the order to the database: {ex.Message}");
    if (!AskToOrderAgain())
    {
        return;
    }
    continue;
}
// End of yhr Save the order to the database BLOCK ///////////////////////////////////////////////

                Console.WriteLine("\n==================================");
                Console.WriteLine("         SHELFY RECEIPT           ");
                Console.WriteLine("==================================");
                Console.WriteLine($"Ticket ID:       {ticketId}");
                Console.WriteLine($"Customer:        {customerName}");
                Console.WriteLine("Items:");
                foreach (var item in cart)
                {
                    Console.WriteLine($"  {item.quantity}x {item.roll.RollName}");
                }
                Console.WriteLine($"Pickup:          {pickupDay.ToUpperInvariant()} @ {pickupTime}");
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Subtotal:        ${subtotal:F2}");
                Console.WriteLine($"Reservation Fee: ${reservationFee:F2}");
                Console.WriteLine($"Tax:             ${tax:F2}");
                Console.WriteLine($"TOTAL PAID:      ${total:F2}");
                Console.WriteLine("==================================");
                Console.WriteLine("Order saved for the Admin dashboard.");

                if (!AskToOrderAgain())
                {
                    return;
                }
            }
        }

        static bool BuildCart(List<(RollOption roll, int quantity)> cart)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=========== SHELFY MENU ===========");
                for (int i = 0; i < AvailableRolls.Count; i++)
                {
                    Console.WriteLine($"{i + 1,2}. {AvailableRolls[i].RollName,-30} ${AvailableRolls[i].Price:F2}");
                }

                if (cart.Count > 0)
                {
                    Console.WriteLine("\nCurrent cart:");
                    foreach (var item in cart)
                    {
                        Console.WriteLine($"  {item.quantity}x {item.roll.RollName}");
                    }
                }

                Console.WriteLine("\nChoose a roll number to add it.");
                Console.WriteLine("0. Checkout" + (cart.Count == 0 ? " (add an item first)" : string.Empty));
                Console.Write("Selection: ");

                if (!int.TryParse(Console.ReadLine(), out int choice)
                    || choice < 0
                    || choice > AvailableRolls.Count)
                {
                    Console.WriteLine("Please enter a valid menu number.");
                    ContinueAfterMessage();
                    continue;
                }

                if (choice == 0)
                {
                    if (cart.Count == 0)
                    {
                        Console.WriteLine("Your cart is empty. Add an item before checkout.");
                        ContinueAfterMessage();
                        continue;
                    }

                    return true;
                }

                RollOption selectedRoll = AvailableRolls[choice - 1];
                Console.Write($"Quantity of {selectedRoll.RollName}: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Enter a quantity greater than zero.");
                    ContinueAfterMessage();
                    continue;
                }

                cart.Add((selectedRoll, quantity));
                Console.WriteLine($"Added {quantity}x {selectedRoll.RollName}.");
                Console.WriteLine("Choose another menu item to add more, or enter 0 to checkout.");
                ContinueAfterMessage();
            }
        }

        static string GetPickupDay()
        {
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            while (true)
            {
                Console.WriteLine("\n--- Order Pickup Day ---");
                for (int i = 0; i < days.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {days[i]}");
                }
                Console.Write("Select pickup day (1-5): ");

                if (int.TryParse(Console.ReadLine(), out int choice)
                    && choice >= 1
                    && choice <= days.Length)
                {
                    return days[choice - 1];
                }

                Console.WriteLine("Please select a day from 1 to 5.");
            }
        }

        static string GetPickupTime()
        {
            while (true)
            {
                Console.Write("\nEnter pickup time (15-minute increments, e.g. 1:15 PM): ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (TimeOnly.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly time)
                    && time >= new TimeOnly(12, 0)
                    && time <= new TimeOnly(20, 0)
                    && time.Minute % 15 == 0)
                {
                    return time.ToString("h:mm tt", CultureInfo.InvariantCulture);
                }

                Console.WriteLine("Enter a time from 12:00 PM to 8:00 PM in 15-minute increments.");
            }
        }

        static void ShowAdminDashboard()
        {
            Console.Clear();
            Console.WriteLine("====== STORE-SIDE LIVE ORDERS ======");
            string ordersPath = GetOrdersFilePath();
            decimal totalRevenue = 0m;

            if (!File.Exists(ordersPath))
            {
                Console.WriteLine("No orders have been saved yet.");
            }
            else
            {
                string[] orders = File.ReadAllLines(ordersPath);
                if (orders.Length == 0)
                {
                    Console.WriteLine("No orders have been saved yet.");
                }

                foreach (string order in orders)
                {
                    Console.WriteLine(order);
                    int lastComma = order.LastIndexOf(',');
                    if (lastComma >= 0
                        && decimal.TryParse(
                            order[(lastComma + 1)..],
                            NumberStyles.Number,
                            CultureInfo.InvariantCulture,
                            out decimal orderTotal))
                    {
                        totalRevenue += orderTotal;
                    }
                }
            }

            Console.WriteLine($"\nTOTAL PLATFORM REVENUE: ${totalRevenue:F2}");
            Console.WriteLine("\nPress Enter to exit.");
            Console.ReadLine();
        }

        static string GetOrdersFilePath()
        {
            DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Shelfy.slnx"))
                    || Directory.Exists(Path.Combine(directory.FullName, "ShelfyWeb")))
                {
                    return Path.Combine(directory.FullName, "orders.csv");
                }

                directory = directory.Parent;
            }

            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "orders.csv"));
        }

        static string FormatCsvField(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        static bool AskYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string answer = Console.ReadLine()?.Trim() ?? string.Empty;

                if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(answer, "yes", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (string.Equals(answer, "n", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(answer, "no", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                Console.WriteLine("Please enter y or n.");
            }
        }

        static bool AskToOrderAgain()
        {
            return AskYesNo("\nWould you like to place another order? (y/n): ");
        }

        static void ContinueAfterMessage()
        {
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
        }
    }
}
