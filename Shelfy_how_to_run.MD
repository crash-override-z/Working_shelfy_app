# Shelfy V1 Project Guide

Welcome to **Shelfy V1**! This repository contains two core components:
* **`ShelfyWeb`**: A web-based admin dashboard.
* **`ShelfyConsole`**: A terminal-based ordering application.

---

## 📁 Directory Navigation Commands

## Boot up port to see changes
dotnet run --urls http://0.0.0.0:5181

## Shelfy V1
 cd "/Users/joshsacrey/Desktop/Shelfy V1" && cd ShelfyWeb
 * dotnet run --project ShelfyConsole

## Shelfy WEB
 cd "/Users/joshsacrey/Desktop/Shelfy V1/ShelfyWeb"
 * dotnet run 

## Passwords
### Local admin demo password
12345678

## Terminal boot-up
cd "/Users/joshsacrey/Desktop/Shelfy V1/ShelfyWeb"
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --urls http://0.0.0.0:5180

## cloudflair
1
## tunnle
cloudflared tunnel --url http://localhost:5180cloudflared tunnel --url http://localhost:5180

The admin password is stored locally with .NET User Secrets, not in this file or in Git.

From the `ShelfyWeb` folder, set or change it with:

```bash
dotnet user-secrets set "AdminPassword" "<your password>"

## User secrets was initialized once for this project with: dotnet user-secrets init


One note: ** Added initialized User Secrets, but haven’t yet confirmed the password was set or finished wiring login to read it.** Will complete those steps as we build the login.











## S H E L F Y =======================================================================================
=======================================================================================

# Project Requirements

The final project is an opportunity to demonstrate the knowledge and skills you have acquired throughout this course. Your project should showcase your ability to design, build, and implement a complete .NET application.

Please review the following requirements carefully.

## **General Requirements**

- The project **must be completed individually**. Group projects are **not permitted**.
- The project **must be written in C# using the .NET 10.0 framework**.
- The project should demonstrate the programming concepts, techniques, and best practices learned during this course.

## **Functional Requirements**

Your application should include **approximately 5 to 7 major features**.

A **feature** represents a significant piece of functionality that provides value to the user. Examples include:

- User registration
- User login and logout
- Viewing a list of products
- Viewing product details
- Creating new products
- Editing existing products
- Deleting products (administrator)
- Managing user profiles
- Adding items to a shopping cart
- Completing a purchase
- Generating invoices
- Searching or filtering records

These are only examples. You are encouraged to design a project that interests you.

## **User Interface Requirements**

Your application should include **a minimum of 5 to 7 screens/pages**.

Examples of screens include:

- Login
- Dashboard
- Product List
- Product Details
- Add/Edit Product
- User Profile
- Administration
- Checkout
- Reports

The exact screens will depend on the type of application you choose to build.

## **Database Requirements**

Your project **must use a database** to store application data.

The application should demonstrate all of the following database operations:

- Reading data from the database
- Creating (inserting) new records
- Updating existing records
- Deleting records where appropriate

In other words, your project should demonstrate full database interaction rather than relying on hard-coded or in-memory data.

## **Authentication and Authorization**

Your application **must include authentication and authorization**.

Specifically, it must provide:

- Login functionality
- Logout functionality
- At least **two different user roles**

For example:

- Administrator and Customer
- Administrator and Employee
- Teacher and Student

Different roles should have access to different functionality within the application.

## **Project Goal**

Your final project should represent a complete, working application that demonstrates your understanding of C#, .NET 10, database development, authentication, authorization, and user interface design.

Choose a project that is interesting to you and that allows you to showcase the skills you have learned during the course.

