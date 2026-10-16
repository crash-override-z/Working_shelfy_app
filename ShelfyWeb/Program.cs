using Microsoft.EntityFrameworkCore;
using Shelfy.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ShelfyDbContext>(options =>
   options.UseSqlite($"Data Source={ShelfyDbContext.GetDatabasePath()}"));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", options =>
{
    options.LoginPath = "/CustomerLogin";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShelfyDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
