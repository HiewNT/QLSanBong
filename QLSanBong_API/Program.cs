using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QLSanBong_API.Data;
using QLSanBong_API.Services;
using QLSanBong_API.Services.IService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext
builder.Services.AddDbContext<QlsanBongContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QlsanBongContext")));

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", builder =>
    {
        builder.WithOrigins("https://localhost:7198", "https://localhost:7182") // URL của ứng dụng client
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

// Cấu hình Authentication với JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],       // Cấu hình trong appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],   // Cấu hình trong appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Khóa từ appsettings.json
    };
});

// Cấu hình Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireEmployeeRole", policy => policy.RequireRole("NhanVien"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("KhachHang"));
});

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Đăng ký các dịch vụ
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ISanBongService, SanBongService>();
builder.Services.AddScoped<INhanVienService, NhanVienService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IGiaGioThueService, GiaGioThueService>();
builder.Services.AddScoped<IPhieuDatSanService, PhieuDatSanService>();
builder.Services.AddScoped<IYeuCauDatSanService, YeuCauDatSanService>();
builder.Services.AddScoped<IGioHangService, GioHangService>();

// Đăng ký dịch vụ MVC
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "QLSanBong API", Version = "v1" });

    // Cấu hình để sử dụng Bearer Token
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập Bearer token vào đây.",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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


var app = builder.Build();

// Cấu hình middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QLSanBong API v1");
        c.RoutePrefix = string.Empty; // Đặt Swagger ở trang chính
    });
}

// Áp dụng chính sách CORS
app.UseCors("MyCors");

// Chuyển hướng HTTPS
app.UseHttpsRedirection();

// Sử dụng Authentication và Authorization
app.UseAuthentication(); // Quan trọng: Sử dụng authentication trước authorization
app.UseAuthorization();

// Sử dụng session
app.UseSession();  // Đảm bảo session được sử dụng

// Map các controller
app.MapControllers();

app.Run();
