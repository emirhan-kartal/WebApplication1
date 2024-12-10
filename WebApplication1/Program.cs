using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication1.Models;
using WebApplication1.Repositories;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IFileRepository, FileRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redirect to login for unauthenticated users
        options.AccessDeniedPath = "/AccessDenied"; // Redirect for unauthorized access
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set cookie expiration

    });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
app.UseAuthorization();



app.Run();
