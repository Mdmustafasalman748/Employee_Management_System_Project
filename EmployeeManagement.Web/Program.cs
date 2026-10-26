using EmployeeManagement.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    db.Database.Migrate();
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

    // Admin account: only created when a password is configured
    var adminPassword = builder.Configuration["Seed:AdminPassword"];
    if (!string.IsNullOrEmpty(adminPassword))
    {
        var admin = await users.FindByEmailAsync("admin@company.com");
        if (admin == null)
        {
            admin = new IdentityUser { UserName = "admin@company.com", Email = "admin@company.com", EmailConfirmed = true };
            await users.CreateAsync(admin, adminPassword);
        }
        if (!await users.IsInRoleAsync(admin, "Admin"))
            await users.AddToRoleAsync(admin, "Admin");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // who are you?   (must come before...)
app.UseAuthorization();    // what may you do?

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();