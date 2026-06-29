using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
using PawdoptApp.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Database + Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath        = "/Account/Login";
    options.LogoutPath       = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly  = true;
    options.ExpireTimeSpan   = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
});

// Lets fetch() calls send the anti-forgery token as a header instead of a form field
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.AddScoped<ApplicationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Ensure DB + seed roles and admin
using (var scope = app.Services.CreateScope())
{
    var db      = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    db.Database.Migrate();

    foreach (var role in new[] { "Admin", "Adopter", "Rehomer" })
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));
    }

    var adminEmail    = builder.Configuration["AdminSeed:Email"]    ?? "admin@pawdopt.ca";
    var adminPassword = builder.Configuration["AdminSeed:Password"] ?? "Admin@2026!";

    if (await userMgr.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName       = adminEmail,
            Email          = adminEmail,
            DisplayName    = "Pawdopt Admin",
            UserType       = "Admin",
            Province       = "QC",
            EmailConfirmed = true,
            CreatedAt      = DateTime.UtcNow
        };
        var result = await userMgr.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userMgr.AddToRoleAsync(admin, "Admin");
            await userMgr.AddClaimsAsync(admin, new[]
            {
                new Claim("DisplayName", admin.DisplayName),
                new Claim("UserType",    admin.UserType)
            });
        }
    }

    // Seed the 4 community "Advisor" profiles as real accounts so visitors can
    // actually message them, instead of the page just disclosing it's a demo.
    var advisors = new[]
    {
        new { Email = "sophie.martin@pawdopt-community.ca",  Name = "Sophie Martin",  City = "Montréal", Province = "QC" },
        new { Email = "marc.tremblay@pawdopt-community.ca",  Name = "Marc Tremblay",  City = "Toronto",  Province = "ON" },
        new { Email = "aisha.johnson@pawdopt-community.ca",  Name = "Aisha Johnson",  City = "Vancouver",Province = "BC" },
        new { Email = "carlos.rivera@pawdopt-community.ca",  Name = "Carlos Rivera",  City = "Calgary",  Province = "AB" }
    };
    foreach (var a in advisors)
    {
        if (await userMgr.FindByEmailAsync(a.Email) != null) continue;

        var advisor = new ApplicationUser
        {
            UserName       = a.Email,
            Email          = a.Email,
            DisplayName    = a.Name,
            UserType       = "Rehomer",
            City           = a.City,
            Province       = a.Province,
            EmailConfirmed = true,
            CreatedAt      = DateTime.UtcNow
        };
        var advResult = await userMgr.CreateAsync(advisor, "Advisor@2026!");
        if (advResult.Succeeded)
        {
            await userMgr.AddToRoleAsync(advisor, "Rehomer");
            await userMgr.AddClaimsAsync(advisor, new[]
            {
                new Claim("DisplayName", advisor.DisplayName),
                new Claim("UserType",    advisor.UserType)
            });
        }
    }
}

app.Run();
