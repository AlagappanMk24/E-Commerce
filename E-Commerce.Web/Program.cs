using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Mappings;
using E_Commerce.Infrastructure.Data.Context;
using E_Commerce.Infrastructure.Services;
using E_Commerce.Web.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("EcomDbConnnection");

builder.Services.AddDbContext<EcomDbContext>(options =>
        options.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(MenuProfile));

builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();