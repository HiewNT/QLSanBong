using Microsoft.EntityFrameworkCore;
using QLSanBong.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<QlsanBongContext>(options =>
    options.UseSqlServer("Data Source=hiew\\nthieu;Initial Catalog=QLSanBong;User ID=sa;Encrypt=false;Trusted_Connection=True;TrustServerCertificate=True;"));  

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
      name: "Admin",
      areaName: "Admin",
      pattern: "Admin/{controller=Home}/{action=Index}"
    );
    endpoints.MapAreaControllerRoute(
      name: "Customer",
      areaName: "Customer",
      pattern: "Customer/{controller=Home}/{action=Index}"
    );
    endpoints.MapAreaControllerRoute(
      name: "Employee",
      areaName: "Employee",
      pattern: "Employee/{controller=Home}/{action=Index}"
    );
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller}/{action}"
    );
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
});



app.Run();
