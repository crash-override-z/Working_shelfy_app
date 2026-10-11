/* using System;
using System.IO; // Required to save files

class Program
{
    static void Main(string[] args)



    {
        Console.WriteLine("--- Sushi Order System ---");

        // 1. Capture the username first!
        Console.Write("Enter your username: ");
        string username = Console.ReadLine().ToLower();

        Console.WriteLine("What would you like to request? Available choices are salmon or tuna.");

        // Prompt user for the item
        Console.Write("Enter roll type: ");
        string rollType = Console.ReadLine().ToLower();

        // Restrict input to only salmon or tuna
        while (rollType != "salmon" && rollType != "tuna")
        {
            Console.WriteLine("Sorry, we only have salmon or tuna available.");
            Console.Write("Please re-enter roll type: ");
            rollType = Console.ReadLine().ToLower();
        }

        // Prompt user for the quantity
        Console.Write("How many rolls would you like? ");
        string quantityInput = Console.ReadLine();

        // Ask for the pickup day
        Console.WriteLine();
        Console.WriteLine("--- Order pickup day ---");
        Console.WriteLine("What day would you like to pick up your order? Available choices are monday, tuesday, wednesday, thursday, or friday.");
        Console.Write("Enter pickup day: ");
        string pickupDay = Console.ReadLine().ToLower();

        // Restrict input to valid days
        while (pickupDay != "monday" && pickupDay != "tuesday" && pickupDay != "wednesday" && pickupDay != "thursday" && pickupDay != "friday")
        {
            Console.WriteLine("Sorry, that is not a valid delivery day.");
            Console.Write("Please enter a valid day: ");
            pickupDay = Console.ReadLine().ToLower();
        }

        // Ask for the pickup time
        Console.WriteLine();
        Console.WriteLine("--- Order pickup time ---");
        Console.WriteLine("What time would you like to pick up your order? Available choices are between 12pm and 8pm.");
        Console.Write("Enter pickup time: ");
        string pickupTime = Console.ReadLine().ToLower();

        // Restrict input to valid hours
        while (pickupTime != "12pm" && pickupTime != "1pm" && pickupTime != "2pm" && pickupTime != "3pm" && pickupTime != "4pm" && pickupTime != "5pm" && pickupTime != "6pm" && pickupTime != "7pm" && pickupTime != "8pm")
        {
            Console.WriteLine("Sorry, we are only open from 12pm to 8pm.");
            Console.Write("Please enter a valid time: ");
            pickupTime = Console.ReadLine().ToLower();
        }

        // Generate a unique ticket number
        string ticketNumber = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

        // Save the order to our backend file (orders.csv) including the username
        string orderData = username + "," + ticketNumber + "," + rollType + "," + quantityInput + "," + pickupDay + "," + pickupTime;
        File.AppendAllText("orders.csv", orderData + Environment.NewLine);

        // Display the final summary back to the user
        Console.WriteLine();
        Console.WriteLine("=== Request Summary ===");
        Console.WriteLine("Ticket Number: " + ticketNumber);
        Console.WriteLine("Item: " + rollType);
        Console.WriteLine("Quantity: " + quantityInput);
        Console.WriteLine("Day: " + pickupDay);
        Console.WriteLine("Time: " + pickupTime);
        Console.WriteLine("\nOrder successfully saved to the central backend!");
    }
}
*/

using System;
using System.Collections.Generic;
using System.IO;

// A simple structure to represent an item in the cart
class CartItem
{
    public string RollName { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }

    public double GetTotalPrice()
    {
        return UnitPrice * Quantity;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Shelfy Order System ---");

        // 1. Capture the username first
        Console.Write("Enter your username (or type 'admin' for store view): ");
        string username = Console.ReadLine().ToLower();

        // 2. Admin Check & Store-Side Dashboard
        if (username == "admin")
        {
            Console.Clear();
            Console.WriteLine("=== STORE-SIDE LIVE ORDERS (ADMIN DASHBOARD) ===");
            
            if (File.Exists("orders.csv"))
            {
                string[] allOrders = File.ReadAllLines("orders.csv");
                if (allOrders.Length == 0)
                {
                    Console.WriteLine("No orders logged yet today.");
                }
                else
                {
                    double totalRevenue = 0;

                    foreach (string order in allOrders)
                    {
                        string[] parts = order.Split(',');
                        if (parts.Length >= 6)
                        {
                            string custUser = parts[0];
                            string ticketId = parts[1];
                            string rollName = parts[2];
                            string day = parts[3];
                            string time = parts[4];
                            double orderTotal = Convert.ToDouble(parts[5]);

                            totalRevenue += orderTotal;

                            Console.WriteLine($"[Ticket: {ticketId}]  Customer: @{custUser}");
                            Console.WriteLine($" └─ Roll(s): {rollName} | Pickup: {day.ToUpper()} @ {time} | Total: ${orderTotal:F2}");
                            Console.WriteLine(new string('-', 50));
                        }
                    }

                    Console.WriteLine($"\nTOTAL PLATFORM REVENUE: ${totalRevenue:F2} (Includes reservation fees)");
                }
            }
            else
            {
                Console.WriteLine("No orders.csv file found yet.");
            }

            Console.WriteLine("\nPress any key to exit store view...");
            Console.ReadKey();
            return;
        }

        // --- MULTI-ITEM CART LIST ---
        List<CartItem> cart = new List<CartItem>();
        bool keepShopping = true;

        // 3. Shopping Loop (Allows adding multiple items)
        while (keepShopping)
        {
            Console.Clear();
            Console.WriteLine("\n--- Fujiya Sushi Menu ---");
            Console.WriteLine("1. Fujiya Roll              $10.95");
            Console.WriteLine("2. Spicy Chopped Tuna Roll   $9.95");
            Console.WriteLine("3. Spicy Chopped Salmon Roll $9.95");
            Console.WriteLine("4. Wild Salmon Maki          $8.50");
            Console.WriteLine("5. Salmon Maki               $6.25");
            Console.WriteLine("6. Tuna Maki                 $6.25");
            Console.WriteLine("7. Salmon Avocado Maki       $8.95");
            Console.WriteLine("8. B.C. Roll                 $6.95");
            Console.WriteLine("9. California Roll           $5.95");
            Console.WriteLine("10. Shrimp California Roll   $8.95");
            Console.Write("Select a roll (1-10): ");

            string rollChoice = Console.ReadLine();
            string rollType = "";
            double unitPrice = 0;

            while (rollType == "")
            {
                switch (rollChoice)
                {
                    case "1": rollType = "Fujiya Roll"; unitPrice = 10.95; break;
                    case "2": rollType = "Spicy Chopped Tuna Roll"; unitPrice = 9.95; break;
                    case "3": rollType = "Spicy Chopped Salmon Roll"; unitPrice = 9.95; break;
                    case "4": rollType = "Wild Salmon Maki"; unitPrice = 8.50; break;
                    case "5": rollType = "Salmon Maki"; unitPrice = 6.25; break;
                    case "6": rollType = "Tuna Maki"; unitPrice = 6.25; break;
                    case "7": rollType = "Salmon Avocado Maki"; unitPrice = 8.95; break;
                    case "8": rollType = "B.C. Roll"; unitPrice = 6.95; break;
                    case "9": rollType = "California Roll"; unitPrice = 5.95; break;
                    case "10": rollType = "Shrimp California Roll"; unitPrice = 8.95; break;
                    default:
                        Console.WriteLine("Invalid choice. Please select between 1 and 10:");
                        rollChoice = Console.ReadLine();
                        break;
                }
            }

            Console.Write($"How many {rollType}s would you like? ");
            int qty = int.Parse(Console.ReadLine());

            // Add item to our cart list
            cart.Add(new CartItem { RollName = rollType, UnitPrice = unitPrice, Quantity = qty });

            // Display current cart contents so the user sees it stacking
            Console.Clear();
            Console.WriteLine("=== CURRENT CART ===");
            foreach (var item in cart)
            {
                Console.WriteLine($"- {item.Quantity}x {item.RollName} (${item.GetTotalPrice():F2})");
            }
            Console.WriteLine("----------------------------------");

            Console.Write("Would you like to add another item or proceed to checkout? (1: Add More, 2: Checkout): ");
            string nextAction = Console.ReadLine().Trim();

            if (nextAction == "2")
            {
                keepShopping = false; 
            }
        }

        // 4. Numbered Pickup Day Selection
        Console.Clear();
        Console.WriteLine("--- Order Pickup Day ---");
        Console.WriteLine("1. Monday");
        Console.WriteLine("2. Tuesday");
        Console.WriteLine("3. Wednesday");
        Console.WriteLine("4. Thursday");
        Console.WriteLine("5. Friday");
        Console.Write("Select pickup day (1-5): ");
        
        string dayChoice = Console.ReadLine();
        string pickupDay = "";

        while (pickupDay == "")
        {
            switch (dayChoice)
            {
                case "1": pickupDay = "monday"; break;
                case "2": pickupDay = "tuesday"; break;
                case "3": pickupDay = "wednesday"; break;
                case "4": pickupDay = "thursday"; break;
                case "5": pickupDay = "friday"; break;
                default:
                    Console.WriteLine("Invalid choice. Select between 1 and 5:");
                    dayChoice = Console.ReadLine();
                    break;
            }
        }

        // 5. Ask for pickup time in 15-minute increments
        Console.Clear();
        Console.WriteLine("--- Order Pickup Time ---");
        Console.WriteLine("Select a 15-minute slot between 12:00 PM and 8:00 PM (e.g., 12:00, 12:15, 12:30):");
        Console.Write("Enter pickup time: ");
        string pickupTime = Console.ReadLine().ToLower();

        string[] validTimes = {
            "12:00", "12:15", "12:30", "12:45",
            "1:00", "1:15", "1:30", "1:45",
            "2:00", "2:15", "2:30", "2:45",
            "3:00", "3:15", "3:30", "3:45",
            "4:00", "4:15", "4:30", "4:45",
            "5:00", "5:15", "5:30", "5:45",
            "6:00", "6:15", "6:30", "6:45",
            "7:00", "7:15", "7:30", "7:45",
            "8:00"
        };

        bool isValidTime = Array.Exists(validTimes, time => time == pickupTime);

        while (!isValidTime)
        {
            Console.WriteLine("Invalid time or not in a 15-minute increment. Try again (e.g., 1:15):");
            pickupTime = Console.ReadLine().ToLower();
            isValidTime = Array.Exists(validTimes, time => time == pickupTime);
        }

        // --- CALCULATE FINANCES FOR ALL ITEMS IN CART ---
        double foodSubtotal = 0;
        foreach (var item in cart)
        {
            foodSubtotal += item.GetTotalPrice();
        }

        double reservationFee = 1.50;
        double taxRate = 0.05; // 5% GST
        double subtotalWithFee = foodSubtotal + reservationFee;
        double estimatedTax = subtotalWithFee * taxRate;
        double grandTotal = subtotalWithFee + estimatedTax;

        // --- CONFIRMATION PROMPT ---
        Console.Clear();
        Console.WriteLine("=== ORDER PREVIEW ===");
        foreach (var item in cart)
        {
            Console.WriteLine($"{item.Quantity}x {item.RollName} @ ${item.UnitPrice:F2} each");
        }
        Console.WriteLine($"Pickup: {pickupDay.ToUpper()} at {pickupTime} PM");
        Console.WriteLine("----------------------------------");
        Console.WriteLine($"Food Subtotal:      ${foodSubtotal:F2}");
        Console.WriteLine($"Reservation Fee:    ${reservationFee:F2}");
        Console.WriteLine($"Estimated Tax (5%): ${estimatedTax:F2}");
        Console.WriteLine($"GRAND TOTAL:        ${grandTotal:F2}");
        Console.WriteLine("----------------------------------");
        
        Console.Write("Do you want to confirm and accept charges? (yes/no): ");
        string confirm = Console.ReadLine()?.Trim().ToLower() ?? string.Empty;

        if (confirm == "yes" || confirm == "y")
        {
            string ticketNumber = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            string itemsSummary = "";
            for (int i = 0; i < cart.Count; i++)
            {
                itemsSummary += $"{cart[i].Quantity}x {cart[i].RollName}";
                if (i < cart.Count - 1) itemsSummary += " + ";
            }

            string orderData = $"{username},{ticketNumber},\"{itemsSummary}\",{pickupDay},{pickupTime},{grandTotal:F2}";
            File.AppendAllText("orders.csv", orderData + Environment.NewLine);

            Console.Clear();
            Console.WriteLine("==================================");
            Console.WriteLine("          SHELFY RECEIPT          ");
            Console.WriteLine("==================================");
            Console.WriteLine($"Ticket ID:       {ticketNumber}");
            Console.WriteLine($"Customer:        {username}");
            Console.WriteLine("Items:");
            foreach (var item in cart)
            {
                Console.WriteLine($"  - {item.Quantity}x {item.RollName}");
            }
            Console.WriteLine($"Pickup Slot:     {pickupDay.ToUpper()} @ {pickupTime} PM");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Subtotal:        ${foodSubtotal:F2}");
            Console.WriteLine($"Reservation Fee: ${reservationFee:F2}");
            Console.WriteLine($"Tax:             ${estimatedTax:F2}");
            Console.WriteLine($"TOTAL PAID:      ${grandTotal:F2}");
            Console.WriteLine("==================================");
            Console.WriteLine("Status: Confirmed & Sent to Kitchen!");
            Console.WriteLine("Thank you for using Shelfy!");
        }
        else
        {
            Console.WriteLine("\nOrder cancelled. Returning safely...");
        }
    }
}
