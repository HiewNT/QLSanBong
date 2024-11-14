using Microsoft.EntityFrameworkCore;
using QLSanBong_API.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container.
builder.Services.AddControllersWithViews();

// Thêm HttpClient để gọi API
builder.Services.AddHttpClient();

// Đăng ký DbContext với connection string từ cấu hình, đảm bảo thông tin kết nối không bị lộ.
builder.Services.AddDbContext<QlsanBongContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QlsanBongContext")));

// Cấu hình MemoryCache cho Distributed Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Domain = ".localhost"; // Chia sẻ session giữa các cổng
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Đảm bảo cookie chỉ gửi qua HTTPS
});

// Cấu hình CORS để chỉ cho phép các domain đáng tin cậy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://localhost:7198", "https://localhost:7182") // URL của ứng dụng client
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials(); // Cho phép gửi cookies
    });
});

// Cấu hình xác thực sử dụng Cookie Authentication (tránh vấn đề bảo mật với xác thực không đủ mạnh)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đảm bảo rằng người dùng phải đăng nhập trước khi truy cập các trang cần bảo mật
    });

var app = builder.Build();

// Cấu hình pipeline HTTP request.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();  // Bắt buộc sử dụng HTTPS
app.UseStaticFiles();

app.UseRouting();

// Áp dụng chính sách CORS
app.UseCors("MyCors");

// Sử dụng session
app.UseSession();

// Sử dụng Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// Định nghĩa các route cho ứng dụng, đảm bảo phân quyền hợp lý cho từng khu vực
app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Admin" });

app.MapControllerRoute(
    name: "Customer",
    pattern: "Customer/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" });

app.MapControllerRoute(
    name: "Employee",
    pattern: "Employee/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Employee" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
