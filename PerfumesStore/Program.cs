using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddLogging();

// Add services to the container.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<PerfumesStore.Service.IEmaiService, EmailService>();
builder.Services.AddScoped<CartService>();


//Configure the DbContext with the connection string
builder.Services.AddDbContext<PerfumesShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapHub<ChatHub>("/chathub"); //sigal R
app.MapRazorPages();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Request.Path = "/Register/Register";
    }
    else if (context.Request.Path == "/Login/login")
    {
        context.Request.Path = "/Login/Login";
    }
    else if (context.Request.Path == "/ForgotPassword/ForgotPassword")
    {
        context.Request.Path = "/ForgotPassword/ForgotPassword";
    }
    else if (context.Request.Path == "/Profile/ViewProfile")
    {
        context.Request.Path = "/Profile/ViewProfile";
    }
    else if (context.Request.Path == "/Profile/EditProfile")
    {
        context.Request.Path = "/Profile/EditProfile";
    }
    else if (context.Request.Path == "/Cart/AddToCart")
    {
        context.Request.Path = "/Cart/AddToCart";
    }
    await next();
    });

app.Run();
