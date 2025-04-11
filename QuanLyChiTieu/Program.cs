using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Thêm session service ở đây (trước khi Build)
builder.Services.AddSession();

// ✅ Thêm DbContext
builder.Services.AddDbContext<QuanLyChiTieuContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ nên thêm dòng này thay vì MapStaticAssets() phía dưới

app.UseRouting();

// ✅ Sử dụng session sau UseRouting, trước Authorization
app.UseSession();

app.UseAuthorization();

// ✅ Map route chuẩn
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

