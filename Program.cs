using Microsoft.EntityFrameworkCore;
using CookBook_Dynamic_Final.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// SQLite
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=cookbook.db";

builder.Services.AddDbContext<CookBookDbContext>(options =>
    options.UseSqlite(connectionString));

// HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Ensure DB exists (fine for SQLite dev)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CookBookDbContext>();
    dbContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
