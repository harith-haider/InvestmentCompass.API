using InvestmentCompass.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة نص الاتصال وقاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. تفعيل سياسة CORS آمنة (Strict Policy)
// 2. تفعيل سياسة CORS آمنة
builder.Services.AddCors(options => {
    options.AddPolicy("SecurePolicy", policy => {
        policy.WithOrigins(
                "https://investment-compass-lemon.vercel.app", // الرابط الجديد مال Vercel
                "http://localhost:5173" // رابط التطوير المحلي
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. إخفاء Swagger في بيئة الإنتاج (لمنع كشف الـ Endpoints للعامة)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. تفعيل سياسة CORS الآمنة
app.UseCors("SecurePolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();