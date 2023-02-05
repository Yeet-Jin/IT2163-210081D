using _210081D_Wong_Yee_Jin.Model;
using _210081D_Wong_Yee_Jin.Services;
using _210081D_Wong_Yee_Jin.ViewModels;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
{
    config.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
}
).AddDefaultTokenProviders().AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDataProtection();
builder.Services.AddReCaptcha(builder.Configuration.GetSection("GoogleReCaptcha"));
builder.Services.AddScoped<AuditLogServices>();
builder.Services.AddScoped<PasswordHistoryService>();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToHRDepartment", policy => policy.RequireClaim("Department", "HR"));
});

builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    Config.SlidingExpiration = true;
    Config.LoginPath = "/Login";
    Config.LogoutPath = "/Logout";
});

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Lockout.MaxFailedAccessAttempts = 3;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    opts.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddTransient<IUserTwoFactorTokenProvider<ApplicationUser>, DataProtectorTokenProvider<ApplicationUser>>();

builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(1));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
