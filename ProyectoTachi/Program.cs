using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using DA.Contexto;
using DA.Implementaciones;
using Flujo;
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
var connString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connString));

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
builder.Services.AddScoped<IFacturaDA, FacturaDA>();
builder.Services.AddScoped<IFacturaFlujo, FacturaFlujo>();
builder.Services.AddScoped<Abstracciones.Interfaces.DA.IClienteDA, DA.Implementaciones.ClienteDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IClienteFlujo, Flujo.ClienteFlujo>();

builder.Services.AddScoped<Abstracciones.Interfaces.DA.IProveedorDA, DA.Implementaciones.ProveedorDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IProveedorFlujo, Flujo.ProveedorFlujo>();

builder.Services.AddScoped<Abstracciones.Interfaces.DA.IPedidoVentaDA, DA.Implementaciones.PedidoVentaDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IPedidoVentaFlujo, Flujo.PedidoVentaFlujo>();

builder.Services.AddScoped<Abstracciones.Interfaces.DA.IFacturaDA, DA.Implementaciones.FacturaDA>();
builder.Services.AddScoped<Abstracciones.Interfaces.Flujo.IFacturaFlujo, Flujo.FacturaFlujo>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IPedidoVentaDA, PedidoVentaDA>();
builder.Services.AddScoped<IPedidoVentaFlujo, PedidoVentaFlujo>();
builder.Services.AddScoped<IProveedorDA, ProveedorDA>();
builder.Services.AddScoped<IProveedorFlujo, ProveedorFlujo>();

var app = builder.Build();
await SeedAdminAsync(app);
await SeedProductsAsync(app);
await SeedClientsAsync(app);
await SeedProductsAsync(app);
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
    
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var renameScript = new System.Text.StringBuilder();
    renameScript.AppendLine("DO $$ DECLARE prop_record RECORD; BEGIN ");
    renameScript.AppendLine("BEGIN ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS \"Stock\" NUMERIC(18,2) NOT NULL DEFAULT 0; EXCEPTION WHEN others THEN NULL; END;");
    foreach (var entity in context.Model.GetEntityTypes())
    {
        var tName = entity.GetTableName();
        if (tName == null || tName.StartsWith("AspNet")) continue;
        
        foreach (var prop in entity.GetProperties())
        {
            var colName = prop.GetColumnName(Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier.Table(tName, null));
            if (colName == null) continue;
            
            renameScript.AppendLine($"BEGIN ALTER TABLE \"{tName}\" RENAME COLUMN {colName.ToLower()} TO \"{colName}\"; EXCEPTION WHEN undefined_column THEN NULL; END;");
        }
    }
    renameScript.AppendLine(@"
CREATE OR REPLACE FUNCTION public.sp_factura_creardesdepedido(p_pedidoventaid integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_factura_id integer;
    v_numero_factura integer;
    v_subtotal decimal(18,2);
    v_impuesto decimal(18,2);
    v_total decimal(18,2);
BEGIN
    SELECT COALESCE(MAX(""NumeroFactura""), 0) + 1 INTO v_numero_factura FROM ""Factura"";

    SELECT COALESCE(SUM(""Cantidad"" * ""PrecioUnitario""), 0) INTO v_subtotal
    FROM ""PedidoVentaDetalle""
    WHERE ""PedidoVentaId"" = p_pedidoventaid;

    v_impuesto := ROUND(v_subtotal * 0.13, 2);
    v_total := v_subtotal + v_impuesto;

    INSERT INTO ""Factura""
        (""NumeroFactura"", ""PedidoVentaId"", ""FechaEmision"", ""Subtotal"", ""Impuesto"", ""Total"", ""Estado"")
    VALUES
        (v_numero_factura, p_pedidoventaid, timezone('utc', now()), v_subtotal, v_impuesto, v_total, 'Emitida')
    RETURNING ""FacturaId"" INTO v_factura_id;

    INSERT INTO ""FacturaDetalle"" (""FacturaId"", ""ProductoId"", ""Cantidad"", ""PrecioUnitario"")
    SELECT v_factura_id, ""ProductoId"", ""Cantidad"", ""PrecioUnitario""
    FROM ""PedidoVentaDetalle""
    WHERE ""PedidoVentaId"" = p_pedidoventaid;

    UPDATE ""Producto"" p
    SET ""Stock"" = COALESCE(p.""Stock"", 0) - d.""Cantidad""
    FROM ""PedidoVentaDetalle"" d
    WHERE p.""ProductoId"" = d.""ProductoId"" AND d.""PedidoVentaId"" = p_pedidoventaid;

    UPDATE ""PedidoVenta"" SET ""Estado"" = 'ENTREGADA'
    WHERE ""PedidoVentaId"" = p_pedidoventaid;

    RETURN v_factura_id;
END;
$function$;

CREATE OR REPLACE FUNCTION public.fn_audit_trigger()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_accion VARCHAR(50);
BEGIN
    IF TG_OP = 'INSERT' THEN
        v_accion := 'INSERT';
    ELSIF TG_OP = 'UPDATE' THEN
        v_accion := 'UPDATE';
    ELSIF TG_OP = 'DELETE' THEN
        v_accion := 'DELETE';
    END IF;

    INSERT INTO ""Auditoria""(tabla, accion, cantidadfilas) 
    VALUES (TG_TABLE_NAME, v_accion, 1);

    RETURN NEW;
END;
$function$;
");
    renameScript.AppendLine("END $$;");
    await context.Database.ExecuteSqlRawAsync(renameScript.ToString());

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

static async Task SeedProductsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. Ensure Category "Ferretería" exists
    var categoria = await context.CategoriasProducto.FirstOrDefaultAsync(c => c.Nombre == "Ferretería");
    if (categoria == null)
    {
        categoria = new Abstracciones.Modelos.CategoriaProductoDto { Nombre = "Ferretería" };
        context.CategoriasProducto.Add(categoria);
        await context.SaveChangesAsync();
    }

    // 2. Check if products already exist to avoid duplication
    if (await context.Productos.AnyAsync()) return;

    // 3. Generate 50 hardware products
    var random = new Random();
    var products = new List<Abstracciones.Modelos.ProductoDto>();
    string[] hardwareItems = { "Martillo", "Destornillador", "Llave Inglesa", "Taladro", "Sierra", "Clavos", "Tornillos", "Tuercas", "Arandelas", "Cinta Métrica", "Nivel", "Alicates", "Llave Allen", "Broca", "Lija", "Pintura", "Brocha", "Rodillo", "Pegamento", "Cemento", "Yeso", "Ladrillo", "Tubería PVC", "Codo PVC", "Grifo", "Válvula", "Cable Eléctrico", "Enchufe", "Interruptor", "Bombilla", "Fusible", "Caja Herramientas", "Guantes", "Gafas Seguridad", "Casco", "Botas", "Escalera", "Carretilla", "Pala", "Rastrillo", "Manguera", "Aspersor", "Maceta", "Tierra", "Semillas", "Abono", "Fertilizante", "Herbicida", "Pesticida", "Raticida" };

    for (int i = 0; i < 50; i++)
    {
        var itemBase = hardwareItems[i % hardwareItems.Length];
        var itemVariant = i / hardwareItems.Length + 1;
        var name = itemVariant > 1 ? $"{itemBase} v{itemVariant}" : itemBase;

        products.Add(new Abstracciones.Modelos.ProductoDto
        {
            SKU = $"FER-{i + 1:000}",
            Nombre = name,
            CategoriaProductoId = categoria.CategoriaProductoId,
            UnidadMedidaId = 1, // Assuming 1 represents "Unidad"
            Costo = random.Next(5, 500),
            Precio = random.Next(10, 1000),
            Stock = random.Next(10, 100),
            StockMinimo = random.Next(5, 20),
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        });
    }

    context.Productos.AddRange(products);
    await context.SaveChangesAsync();
}

static async Task SeedClientsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. Check if clients already exist
    if (await context.Clientes.AnyAsync()) return;

    // 2. Generate 50 clients
    var random = new Random();
    var clients = new List<Abstracciones.Modelos.ClienteDto>();
    string[] firstNames = { "Juan", "María", "Pedro", "Ana", "Luis", "Carmen", "José", "Laura", "Carlos", "Sofía", "Miguel", "Elena", "Francisco", "Isabel", "David", "Patricia", "Manuel", "Lucía", "Javier", "Teresa" };
    string[] lastNames = { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Gómez", "Martín", "Jiménez", "Ruiz", "Hernández", "Díaz", "Moreno", "Muñoz", "Álvarez", "Romero", "Alonso", "Gutiérrez" };

    for (int i = 0; i < 50; i++)
    {
        var firstName = firstNames[random.Next(firstNames.Length)];
        var lastName = lastNames[random.Next(lastNames.Length)];
        var fullName = $"{firstName} {lastName}";

        clients.Add(new Abstracciones.Modelos.ClienteDto
        {
            NombreCompleto = fullName,
            Identificacion = $"{random.Next(100000000, 999999999)}", // Random 9-digit ID
            Correo = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com",
            Telefono = $"{random.Next(60000000, 99999999)}",
            Direccion = $"Calle {random.Next(1, 100)}, Av {random.Next(1, 20)}",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        });
    }

    context.Clientes.AddRange(clients);
    await context.SaveChangesAsync();
}
