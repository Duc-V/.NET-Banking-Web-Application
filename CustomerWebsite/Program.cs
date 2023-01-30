using Assignment2.Data;
using Microsoft.EntityFrameworkCore;
using Assignment2.Controllers;
using SimpleHashing.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(McbaContext)));

    //Enable lazy loading.
    options.UseLazyLoadingProxies();
});

// Store session into Web-Server memory.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});
// Add people background service to automatically run in the background along-side the web-server.
builder.Services.AddHostedService<BillPaymentService>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISimpleHash, SimpleHash>();

var app = builder.Build();

//Seed data.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapDefaultControllerRoute();
app.MapControllerRoute("default", "{controller=Home}/{action=Login}");

app.Run();
