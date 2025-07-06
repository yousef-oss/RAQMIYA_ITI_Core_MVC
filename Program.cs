using ITI_Raqmiya_MVC.Data;
using ITI_Raqmiya_MVC.Repository.Repos_Implementation;
using ITI_Raqmiya_MVC.Repository.Repository_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<RaqmiyaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

builder.Services.AddScoped<IOrder, OrderRepo>();

builder.Services.AddScoped<IProductRepo, ProductRepo>();

builder.Services.AddScoped<IUserRepo, UserRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
