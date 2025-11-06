using Cine_Lumia.Data; // para el seeder
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CADENA DE CONEXIÓN
// ============================
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ============================
// CONFIGURACIÓN DE DbContext
// ============================
builder.Services.AddDbContext<CineDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)) // 🔹 evita cartesian explosion
);

// ============================
// MVC
// ============================
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// ============================
// AUTENTICACIÓN Y COOKIES
// ============================
builder.Services.AddAuthentication("LumiaCookieAuth")
    .AddCookie("LumiaCookieAuth", options =>
    {
        options.Cookie.Name = "LumiaCookieAuth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(1); 
    });

var app = builder.Build();

// ============================
// MIDDLEWARES
// ============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ============================
// SEEDING Y MIGRACIONES
// ============================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CineDbContext>();
    dbContext.Database.Migrate();              // 🔹 Aplica migraciones automáticamente
    CineSeeder.Seed(dbContext);                // 🔹 Carga datos iniciales si no existen
}

// ============================
// EJECUCIÓN
// ============================
app.Run();
