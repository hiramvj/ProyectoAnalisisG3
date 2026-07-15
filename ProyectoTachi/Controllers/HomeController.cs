using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoTachi.Models;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace ProyectoTachi.Controllers
{
    [Authorize(Roles = "Admin,Ventas,Operaciones,Gerencia")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoFlujo _productoFlujo;
        private readonly IClienteFlujo _clienteFlujo;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, 
            IProductoFlujo productoFlujo, 
            IClienteFlujo clienteFlujo,
            AppDbContext db)
        {
            _logger = logger;
            _productoFlujo = productoFlujo;
            _clienteFlujo = clienteFlujo;
            _db = db;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return View(new DashboardViewModel());
            }

            // Producto Metrics
            var productosActivos = await _productoFlujo.ObtenerTodosAsync(true);
            var productosInactivos = await _productoFlujo.ObtenerTodosAsync(false);
            
            var totalProductos = productosActivos.Count + productosInactivos.Count;
            // Assuming we care about low stock mainly for active items
            var bajoStock = productosActivos.Count(p => p.Stock <= p.StockMinimo);

            // Cliente Metrics
            var clientesActivos = await _clienteFlujo.ObtenerTodosAsync(true);
            var clientesInactivos = await _clienteFlujo.ObtenerTodosAsync(false);

            var model = new DashboardViewModel
            {
                TotalProductos = totalProductos,
                ProductosBajoStock = bajoStock,
                TotalClientes = clientesActivos.Count + clientesInactivos.Count,
                ClientesActivos = clientesActivos.Count
            };

            var hoy = DateTime.UtcNow.Date;
            var manana = hoy.AddDays(1);
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            if (User.IsInRole("Admin") || User.IsInRole("Ventas"))
            {
                model.PedidosHoy = await _db.PedidoVentas.CountAsync(p => p.FechaPedido >= hoy && p.FechaPedido < manana);
                model.FacturasHoy = await _db.Facturas.CountAsync(f => f.FechaEmision >= hoy && f.FechaEmision < manana);
                model.VentasHoy = await _db.Facturas
                    .Where(f => f.FechaEmision >= hoy && f.FechaEmision < manana)
                    .SumAsync(f => (decimal?)f.Total) ?? 0;
            }

            if (User.IsInRole("Admin") || User.IsInRole("Operaciones"))
            {
                model.OrdenesPendientes = await _db.OrdenesCompra
                    .CountAsync(o => o.Estado != "COMPLETADA" && o.Estado != "CANCELADA");
                model.ProveedoresActivos = await _db.Proveedores.CountAsync(p => p.Activo);
                model.RutasActivas = await _db.RutasEntrega
                    .CountAsync(r => r.Estado != "COMPLETADA" && r.Estado != "CANCELADA");
            }

            if (User.IsInRole("Admin") || User.IsInRole("Gerencia"))
            {
                model.VentasMes = await _db.Facturas
                    .Where(f => f.FechaEmision >= inicioMes && f.FechaEmision < manana)
                    .SumAsync(f => (decimal?)f.Total) ?? 0;
                model.SaldoPorPagar = await _db.CuentasPorPagar.SumAsync(c => (decimal?)c.SaldoPendiente) ?? 0;
                model.CuentasVencidas = await _db.CuentasPorPagar
                    .CountAsync(c => c.SaldoPendiente > 0 && c.FechaVencimiento < hoy);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,Gerencia")]
        public async Task<IActionResult> InformeFinanciero()
        {
            // Usamos _productoFlujo que ya está inyectado en tu controlador
            var productos = await _productoFlujo.ObtenerTodosAsync(true);

            var model = new InformeFinancieroViewModel();

            if (productos != null && productos.Any())
            {
                model.ValorTotalCosto = productos.Sum(p => p.Costo * p.Stock);
                model.ValorTotalVenta = productos.Sum(p => p.Precio * p.Stock);
                model.MargenGananciaEstimado = model.ValorTotalVenta - model.ValorTotalCosto;

                model.Productos = productos.Select(p => new DetalleFinancieroProducto
                {
                    Nombre = p.Nombre,
                    Cantidad = p.Stock,
                    PrecioVenta = p.Precio,
                    SubtotalVenta = p.Precio * p.Stock
                }).ToList();
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Gerencia")]
        public async Task<IActionResult> GenerarNota(NotaContableDto nota)
        {
            if (nota.Monto <= 0)
            {
                ModelState.AddModelError("", "El monto debe ser mayor a cero.");
                return View(nota);
            } 

            return RedirectToAction("HistorialVentas");
        }

    }
}
