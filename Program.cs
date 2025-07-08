using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Repository.Repos_Implementation;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RaqmiyaContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

// Repo Registeration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // NEW
builder.Services.AddScoped<ITagRepository, TagRepository>(); // NEW


//builder.Services.AddScoped<IOrder, OrderRepo>();

//builder.Services.AddScoped<IProductRepository, ProductRepository>();

//builder.Services.AddScoped<IUserRepo, UserRepo>();

// DIC injection for AuthRepository

builder.Services.AddSession();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>

    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
