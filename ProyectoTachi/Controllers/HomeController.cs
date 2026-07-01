using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoTachi.Models;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;

namespace ProyectoTachi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoFlujo _productoFlujo;
        private readonly IClienteFlujo _clienteFlujo;

        public HomeController(ILogger<HomeController> logger, 
            IProductoFlujo productoFlujo, 
            IClienteFlujo clienteFlujo)
        {
            _logger = logger;
            _productoFlujo = productoFlujo;
            _clienteFlujo = clienteFlujo;
        }

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

            return View(model);
        }

        [Authorize]
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
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
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
