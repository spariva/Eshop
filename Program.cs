using Eshop.Data;
using Eshop.Helpers;
using Eshop.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

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

//System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
//customCulture.NumberFormat.NumberDecimalSeparator = ".";

//System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

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

//UseHttpsRedirection() should be early to ensure all requests are secure
//UseStaticFiles() comes early to efficiently serve static content without processing
//UseRequestLocalization() should be after static files but before other processing
//UseSession() should be set up before routing since components might need session data
//UseRouting() needs to come before authorization so the routing system can determine which endpoint will be executed
//UseAuthorization() depends on routing information to apply the correct authorization rules
//MapStaticAssets() and other endpoint mapping should be at the end

app.UseHttpsRedirection();


app.UseStaticFiles();


app.UseRequestLocalization();


app.UseSession();


app.UseRouting();


app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}")
    .WithStaticAssets();


app.Run();
