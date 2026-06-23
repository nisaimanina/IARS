using Microsoft.EntityFrameworkCore;
using IARS.Data;
using IARS.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. SERVICES SECTION (Pendaftaran Dependency Injection)
// ---------------------------------------------------------

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// --- UPDATE: Susunan Service Penting ---
// Daftarkan HttpClient untuk AIService terlebih dahulu
builder.Services.AddHttpClient<AIService>(); 

// Daftarkan KaizenService (Gunakan AddScoped supaya satu instance setiap request)
builder.Services.AddScoped<KaizenService>();

// Daftarkan NotificationService
builder.Services.AddScoped<NotificationService>();

// Session configuration (Diperlukan untuk EmployeeID login)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ---------------------------------------------------------
// 2. MIDDLEWARE SECTION (Turutan Execution Pipeline)
// ---------------------------------------------------------

// Semak mode pembangunan (Development vs Production)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Jika ralat berlaku di production, pergi ke Error page
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- UPDATE: Session MESTI sebelum Authorization ---
app.UseSession(); // // Penting: Pastikan ini sebelum Authorization!
app.UseAuthorization();

app.MapControllers();

// Routing default (Aplikasi bermula di Login Page)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

// --- LINE 66: Titik Permulaan Aplikasi ---
// Jika crash di sini, semak Database Connection atau Dependency Injection
app.Run();