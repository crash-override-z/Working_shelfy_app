# Shelfy — Community-Powered Local Availability

## Shelfy 
Helps people check what’s actually available at nearby stores before making a trip. Customers can quickly share whether a product is in stock, out of stock, or freshly available, helping other shoppers make better decisions and helping retailers understand local demand.

## The long-term idea
 To make these updates quick and useful for everyone: customers get more reliable shelf information and may earn rewards for helpful contributions; retailers get local demand signals and can attract customers when products are available. Optional reservations or pre-orders could also help stores plan and reduce waste.

## Course Project: A Single-Store Pilot

For the final project, Shelfy is being developed as a focused version for one retailer: Fujiya in Victoria, initially centered on its sushi offerings. This smaller scope gives the project a practical way to demonstrate the core idea and build toward a larger multi-store service.

The current implementation includes a web ordering form where customers select sushi items and pickup details. Orders and their individual items are saved to SQLite, and a store-side dashboard lets staff view orders and update their status.

## Current Development Status

**Implemented:**
- Customer web form for selecting sushi rolls, quantities, and pickup time.
- Server-side validation and calculation of order totals.
- SQLite persistence for orders and their item details.
- Admin order list with status updates and revenue totals.

**Planned next:**
- Make product availability visible for the Fujiya pilot.
- Let customers contribute quick in-stock or out-of-stock updates.
- Show useful availability and demand information to store staff.
- Add authentication and customer/admin roles.
- Explore rewards and other features from the broader Shelfy vision.

Shelfy is being built with C#, ASP.NET Core Razor Pages, Entity Framework Core, and SQLite on .NET 8 for the course project.