using Microsoft.EntityFrameworkCore;
  using CookBook_Dynamic_Final.Data;

  var builder = WebApplication.CreateBuilder(args);

  // MVC
  builder.Services.AddControllersWithViews();

  // SQLite Connection - Different paths for local vs Azure
  var connectionString =
  builder.Configuration.GetConnectionString("DefaultConnection");

  if (string.IsNullOrEmpty(connectionString))
  {
      if (builder.Environment.IsProduction())
      {
          // Azure: MUST use /home/data for persistent storage
          connectionString = "Data Source=/home/data/cookbook.db";
      }
      else
      {
          // Local development: project root
          connectionString = "Data Source=cookbook.db";
      }
  }

  builder.Services.AddDbContext<CookBookDbContext>(options =>
      options.UseSqlite(connectionString));

  // HttpClient for API calls
  builder.Services.AddHttpClient();

  var app = builder.Build();

  // Database initialization - runs on every startup
  using (var scope = app.Services.CreateScope())
  {
      var dbContext =
  scope.ServiceProvider.GetRequiredService<CookBookDbContext>();

      // Create persistent data directory on Azure
      if (app.Environment.IsProduction())
      {
          var dbDirectory = "/home/data";
          if (!Directory.Exists(dbDirectory))
          {
              Directory.CreateDirectory(dbDirectory);
          }
      }

      // Create database and tables if they don't exist
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
