using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoTachi.Models;
using Abstracciones.Interfaces.Flujo;

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
