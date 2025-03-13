using Eshop.Data;
using Eshop.Helpers;
using Eshop.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});


builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddSingleton<HelperToolkit>();
builder.Services.AddSingleton<HelperCriptography>();

//string connectionString =
//    builder.Configuration.GetConnectionString("SqlClase");
string connectionString =
    builder.Configuration.GetConnectionString("SqlCasa");

builder.Services.AddDbContext<EshopContext>
    (options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<RepositoryUsers>();
builder.Services.AddTransient<RepositoryStores>();


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

app.UseStaticFiles();

app.UseRequestLocalization();

app.UseSession();

app.UseRouting();

app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());


app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}")
    .WithStaticAssets();


app.Run();
