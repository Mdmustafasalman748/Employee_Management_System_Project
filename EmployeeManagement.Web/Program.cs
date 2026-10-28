using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using EmployeeManagement.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=employees.db";
var usePostgres = DbProvider.IsPostgres(connectionString);

if (usePostgres)
{
    // Lets Npgsql accept the plain DateTime values used in this app
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    connectionString = DbProvider.ToNpgsql(connectionString);
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (usePostgres) options.UseNpgsql(connectionString);
    else options.UseSqlite(connectionString);
});

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (usePostgres)
    {
        // The existing migrations were generated for SQLite, so on PostgreSQL we
        // create the tables straight from the model. EnsureCreated() skips everything
        // if the database already holds any table, so we call CreateTables() directly
        // and ignore "already exists" on later starts.
        var creator = db.GetService<IRelationalDatabaseCreator>();
        try { creator.CreateTables(); }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateTable) { }
    }
    else
    {
        db.Database.Migrate();
    }

    DbSeeder.Seed(db);

    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roles.RoleExistsAsync(role))
            await roles.CreateAsync(new IdentityRole(role));
    }

    var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Demo account: a normal User, so visitors can add/edit but not delete
    var demo = await users.FindByEmailAsync("demo@company.com");
    if (demo == null)
    {
        demo = new IdentityUser { UserName = "demo@company.com", Email = "demo@company.com", EmailConfirmed = true };
        await users.CreateAsync(demo, "Demo@12345");
    }
    if (!await users.IsInRoleAsync(demo, "User"))
        await users.AddToRoleAsync(demo, "User");

    // Admin account: created, or its password updated, when a password is configured
    var adminPassword = builder.Configuration["Seed:AdminPassword"];
    if (!string.IsNullOrEmpty(adminPassword))
    {
        var admin = await users.FindByEmailAsync("admin@company.com");
        if (admin == null)
        {
            admin = new IdentityUser { UserName = "admin@company.com", Email = "admin@company.com", EmailConfirmed = true };
            await users.CreateAsync(admin, adminPassword);
        }
        else if (!await users.CheckPasswordAsync(admin, adminPassword))
        {
            var token = await users.GeneratePasswordResetTokenAsync(admin);
            await users.ResetPasswordAsync(admin, token, adminPassword);
        }

        if (!await users.IsInRoleAsync(admin, "Admin"))
            await users.AddToRoleAsync(admin, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    // In production the hosting platform handles HTTPS in front of the container
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();