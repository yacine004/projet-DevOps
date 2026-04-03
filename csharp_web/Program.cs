using csharp_web.Data;
using csharp_web.Repositories;
using csharp_web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== MVC =====
builder.Services.AddControllersWithViews();

// ===== CONNECTION STRING =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ===== DbContext =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ===== SESSION =====
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===== REPOSITORIES =====
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();
builder.Services.AddScoped<ILivreurRepository, LivreurRepository>();

// ===== SERVICES =====
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<ILivreurService, LivreurService>();
builder.Services.AddScoped<IPanierService, PanierService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ===== MIGRATIONS AUTOMATIQUES =====
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        csharp_web.Data.DbInitializer.Initialize(context);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Erreur lors de la migration/initialisation : " + ex.Message);
}

// ===== PIPELINE HTTP =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
