using DA.Contexto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoTachi.Servicios;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Pages (Identity UI usa Razor Pages)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity 
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddScoped<Abstracciones.Interfaces.DA.IProductoDA, DA.Implementaciones.ProductoDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IProductoFlujo, Flujo.ProductoFlujo>();

builder.Services.AddScoped<Abstracciones.Interfaces.DA.IClienteDA, DA.Implementaciones.ClienteDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IClienteFlujo, Flujo.ClienteFlujo>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();
await SeedAdminAsync(app);
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string roleName = "Admin";
    var adminEmail = "admin@tachi.com";
    var adminPass = "Admin123*";

    // 1) Crear rol si no existe
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (!roleResult.Succeeded)
            throw new Exception("No se pudo crear el rol Admin: " +
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }

    // 2) Buscar usuario en BD
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // 3) Si no existe, crearlo y verificar resultado
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPass);

        if (!createResult.Succeeded)
        {
            // IMPORTANTISIMO: si falla, NO sigas a asignar roles
            throw new Exception("No se pudo crear el usuario admin: " +
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }

        // 4) Re-cargar desde BD para asegurar que existe y tiene Id real
        adminUser = await userManager.FindByEmailAsync(adminEmail);
    }

    // 5) Asignar rol solo si el usuario existe en BD
    if (!await userManager.IsInRoleAsync(adminUser!, roleName))
    {
        var addRoleResult = await userManager.AddToRoleAsync(adminUser!, roleName);
        if (!addRoleResult.Succeeded)
            throw new Exception("No se pudo asignar el rol Admin: " +
                string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string roleName = "Admin";
    const string adminEmail = "admin@tachi.com";
    const string adminPass = "Admin123*";

    if (!await roleManager.RoleExistsAsync(roleName))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (!roleResult.Succeeded)
            throw new Exception("No se pudo crear el rol Admin: " +
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPass);

        if (!createResult.Succeeded)
            throw new Exception("No se pudo crear el usuario admin: " +
                string.Join(", ", createResult.Errors.Select(e => e.Description)));

        adminUser = await userManager.FindByEmailAsync(adminEmail);
    }

    if (adminUser == null)
        throw new Exception("El usuario admin no quedó creado correctamente.");

    if (!await userManager.IsInRoleAsync(adminUser, roleName))
    {
        var addRoleResult = await userManager.AddToRoleAsync(adminUser, roleName);
        if (!addRoleResult.Succeeded)
            throw new Exception("No se pudo asignar el rol Admin: " +
                string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
    }
}
