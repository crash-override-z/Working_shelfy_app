using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ShelfyConsole
{
    class Program
    {
        class RollOption
        {
            public string RollName { get; set; }
            public decimal Price { get; set; }

            public RollOption(string name, decimal price)
            {
                RollName = name;
                Price = price;
            }
        }

        static void Main(string[] args)
        {
            bool keepRunning = true;

            while (keepRunning)
            {
                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("         SHELFY CHECKOUT          ");
                Console.WriteLine("==================================\n");

                Console.Write("Enter Customer Name: ");
                string customerName = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    customerName = "Guest";
                }

                List<RollOption> availableRolls = new List<RollOption>()
                {
                    new RollOption("Fujiya Roll", 10.95m),
                    new RollOption("Spicy Tuna Roll", 12.50m),
                    new RollOption("California Roll", 8.75m),
                    new RollOption("Dynamite Roll", 11.25m),
                    new RollOption("Salmon Avocado Roll", 9.50m)
                };

                List<(RollOption roll, int quantity)> cart = new List<(RollOption, int)>();
                bool ordering = true;

                while (ordering)
                {
                    Console.WriteLine("\nAvailable Menu Items:");
                    for (int i = 0; i < availableRolls.Count; i++)
                    {
                        Console.WriteLine($"  {i + 1}. {availableRolls[i].RollName} - ${availableRolls[i].Price:F2}");
                    }
                    Console.WriteLine("  0. Finish Order & Checkout");

                    Console.Write("\nSelect an item number (0-{0}): ", availableRolls.Count);
                    string menuInput = Console.ReadLine() ?? string.Empty;

                    if (int.TryParse(menuInput, out int menuChoice))
                    {
                        if (menuChoice == 0)
                        {
                            if (cart.Count == 0)
                            {
                                Console.WriteLine("\nYour cart is empty! Please add at least one item.");
                                continue;
                            }
                            ordering = false;
                        }
                        else if (menuChoice >= 1 && menuChoice <= availableRolls.Count)
                        {
                            RollOption selectedRoll = availableRolls[menuChoice - 1];

                            Console.Write($"Enter quantity for {selectedRoll.RollName}: ");
                            string qtyInput = Console.ReadLine() ?? string.Empty;

                            if (int.TryParse(qtyInput, out int quantity) && quantity > 0)
                            {
                                cart.Add((selectedRoll, quantity));
                                Console.WriteLine($"Added {quantity}x {selectedRoll.RollName} to your order.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid quantity. Please enter a number greater than 0.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection. Please choose a valid menu number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                    }
                }

                string[] pickupSlots = {
                    "MONDAY @ 12:45 PM",
                    "MONDAY @ 1:15 PM",
                    "TUESDAY @ 12:30 PM",
                    "WEDNESDAY @ 1:00 PM"
                };

                Console.WriteLine("\nSelect a Pickup Slot:");
                for (int i = 0; i < pickupSlots.Length; i++)
                {
                    Console.WriteLine($"  {i + 1}. {pickupSlots[i]}");
                }

                int slotIndex = -1;
                while (slotIndex < 0 || slotIndex >= pickupSlots.Length)
                {
                    Console.Write($"Select slot (1-{pickupSlots.Length}): ");
                    if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= pickupSlots.Length)
                    {
                        slotIndex = choice - 1;
                    }
                    else
                    {
                        Console.WriteLine("Invalid slot selection. Try again.");
                    }
                }
                string pickupSlot = pickupSlots[slotIndex];

                decimal subtotal = 0m;
                foreach (var item in cart)
                {
                    subtotal += item.roll.Price * item.quantity;
                }

                decimal reservationFee = 1.50m;
                decimal tax = subtotal * 0.05m;
                decimal totalPaid = subtotal + reservationFee + tax;
                string ticketId = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

                Console.Clear();
                Console.WriteLine("==================================");
                Console.WriteLine("         SHELFY RECEIPT           ");
                Console.WriteLine("==================================");
                Console.WriteLine($"Ticket ID:       {ticketId}");
                Console.WriteLine($"Customer:        {customerName}");
                Console.WriteLine("Items:");
                foreach (var item in cart)
                {
                    Console.WriteLine($"  {item.quantity}x {item.roll.RollName}");
                }
                Console.WriteLine($"Pickup Slot:     {pickupSlot}");
                Console.WriteLine("----------------------------------");
                Console.WriteLine($"Subtotal:        ${subtotal:F2}");
                Console.WriteLine($"Reservation Fee: ${reservationFee:F2}");
                Console.WriteLine($"Tax:             ${tax:F2}");
                Console.WriteLine($"TOTAL PAID:      ${totalPaid:F2}");
                Console.WriteLine("==================================");
                Console.WriteLine("Status: Confirmed & Sent to Kitchen!");
                Console.WriteLine("Thank you for using Shelfy!");

                string[] pickupParts = pickupSlot.Split(" @ ", 2, StringSplitOptions.None);
                string itemsSummary = string.Join(
                    " + ",
                    cart.Select(item => $"{item.quantity}x {item.roll.RollName}"));
                string orderData = string.Join(
                    ",",
                    new[]
                    {
                        customerName,
                        ticketId,
                        itemsSummary,
                        pickupParts[0].ToLowerInvariant(),
                        pickupParts[1],
                        totalPaid.ToString("F2", CultureInfo.InvariantCulture)
                    }.Select(FormatCsvField));

                try
                {
                    File.AppendAllText(GetOrdersFilePath(), orderData + Environment.NewLine);
                    Console.WriteLine("\nOrder saved to the Admin dashboard.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[Warning: Could not save order for the Admin dashboard: {ex.Message}]");
                }

                Console.Write("\nWould you like to place another order? (y/n): ");
                string again = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.Equals(again, "y", StringComparison.OrdinalIgnoreCase))
                {
                    keepRunning = false;
                }
            }
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
    }
}