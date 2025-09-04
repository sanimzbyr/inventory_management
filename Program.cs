using Inventoria.Data;
using Inventoria.Hubs;
using Inventoria.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------ Services ------------

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity (local-dev friendly)
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;    // turn ON email confirm later
        // options.Password.RequireNonAlphanumeric = false; // optional dev relax
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Identity/Account/Login";
    o.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication(); // base cookie scheme

// OPTIONAL externals (only if secrets are present)
var googleId = builder.Configuration["Authentication:Google:ClientId"];
var googleSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleId) && !string.IsNullOrWhiteSpace(googleSecret))
{
    builder.Services.AddAuthentication().AddGoogle(o =>
    {
        o.ClientId = googleId!;
        o.ClientSecret = googleSecret!;
    });
}

var fbId = builder.Configuration["Authentication:Facebook:AppId"];
var fbSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
if (!string.IsNullOrWhiteSpace(fbId) && !string.IsNullOrWhiteSpace(fbSecret))
{
    builder.Services.AddAuthentication().AddFacebook(o =>
    {
        o.AppId = fbId!;
        o.AppSecret = fbSecret!;
    });
}

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();      // Identity UI needs this
builder.Services.AddSignalR();         // required because we MapHub

// Helpful EF/DB error pages in Development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// ------------ Pipeline ------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();   // surfaces DB & migration issues nicely
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // MUST be before authorization
app.UseAuthorization();

// SignalR hub
app.MapHub<DiscussionHub>("/hubs/discussion");

// MVC + Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // maps /Identity/Account/* (Login/Register, etc.)

app.Run();
