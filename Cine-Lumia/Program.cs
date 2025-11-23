using Cine_Lumia.Data;
using Cine_Lumia.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ============================
// CADENA DE CONEXIÓN
// ============================
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'DefaultConnection' was not found.");
}


// ============================
// CONFIGURACIÓN DE DbContext
// ============================
builder.Services.AddDbContext<CineDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
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

// 🔹 Mover esto ANTES de `await next()` y antes de `UseRouting`
app.Use(async (context, next) =>
{
    // Evita que las páginas protegidas se guarden en caché
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "-1";

    await next();
});

app.UseStaticFiles();
app.MapStaticAssets();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

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
    dbContext.Database.Migrate();
    CineSeeder.Seed(dbContext);
}

// ============================
// EJECUCIÓN
// ============================
app.Run();
