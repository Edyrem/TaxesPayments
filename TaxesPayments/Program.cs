using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaxesPayments.Data;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<TaxesPaymentsContext>(options =>
//    //options.UseSqlServer(builder.Configuration.GetConnectionString("TaxesPaymentsContext") ?? throw new InvalidOperationException("Connection string 'TaxesPaymentsContext' not found.")));
//    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection") ?? throw new InvalidOperationException("Connection string 'TaxesPaymentsContext' not found.")));

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("RemoteConnection");
builder.Services.AddDbContext<TaxesPaymentsContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TaxesPaymentsContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
