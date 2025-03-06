using Eshop.Data;
using Eshop.Helpers;
using Eshop.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddSingleton<HelperToolkit>();
builder.Services.AddSingleton<HelperCriptography>();

//string connectionString =
//    builder.Configuration.GetConnectionString("SqlClase");
string connectionString =
    builder.Configuration.GetConnectionString("SqlCasa");

builder.Services.AddTransient<RepositoryUsers>();
builder.Services.AddTransient<RepositoryStores>();


builder.Services.AddDbContext<EshopContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddAntiforgery();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();
app.MapStaticAssets();

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}")
    .WithStaticAssets();


app.Run();
