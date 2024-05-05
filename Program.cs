using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FP._059_NAGASystems_Prod4.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FP_059_NAGASystems_Prod4Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FP_059_NAGASystems_Prod4Context") ?? throw new InvalidOperationException("Connection string 'FP_059_NAGASystems_Prod4Context' not found.")));

// Agrega servicios al contenedor.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Process.Start(new ProcessStartInfo("cmd", $"/c start http://localhost:{builder.Configuration["ApplicationPort"]}") { CreateNoWindow = true });

app.Run();
