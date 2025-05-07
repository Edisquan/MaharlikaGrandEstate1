using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaharlikaGrandEstate.Data;
using MaharlikaGrandEstate.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using MaharlikaGrandEstate.Services;



var builder = WebApplication.CreateBuilder(args);

// 1. Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure Identity with Lockout + Password Rules
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ───────────── ADVANCED PASSWORD RULES ─────────────
    options.Password.RequireDigit = true;         // Must contain at least one digit
    options.Password.RequiredLength = 8;          // Minimum length
    options.Password.RequireNonAlphanumeric = false; // Set to true if you need symbols
    options.Password.RequireUppercase = true;     // Must contain uppercase letter
    options.Password.RequireLowercase = true;     // Must contain lowercase letter

    // ───────────── LOCKOUT FEATURE ─────────────
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders();

// 3. Register Services for Dependency Injection
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<IEmailSender, GmailEmailSender>(); // Email service
builder.Services.AddScoped<StripeService>();


// 4. Configure Stripe Payment Gateway
var stripeSettingsSection = builder.Configuration.GetSection("Stripe");
builder.Services.Configure<StripeSettings>(stripeSettingsSection);

var stripeSettings = stripeSettingsSection.Get<StripeSettings>()
    ?? throw new InvalidOperationException("Stripe configuration is missing or invalid.");

if (string.IsNullOrWhiteSpace(stripeSettings.SecretKey))
{
    throw new InvalidOperationException("Stripe SecretKey is missing in configuration.");
}

StripeConfiguration.ApiKey = stripeSettings.SecretKey;


// 5. Add Controllers + Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 6. Middleware Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// 7. (Optional) Create an Admin user if none exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Ensure the Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Create admin user if not found
    string adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin User"
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Use a secure password
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();
