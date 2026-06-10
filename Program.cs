using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
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
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
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

    db.Database.EnsureCreated();

    foreach (var role in new[] { "Admin", "Adopter", "Rehomer" })
    {
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));
    }

    const string adminEmail = "admin@pawdopt.ca";
    if (await userMgr.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName     = adminEmail,
            Email        = adminEmail,
            DisplayName  = "Pawdopt Admin",
            UserType     = "Admin",
            Province     = "QC",
            EmailConfirmed = true,
            CreatedAt    = DateTime.UtcNow
        };
        var result = await userMgr.CreateAsync(admin, "Admin@2026!");
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
}

app.Run();
