using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly IFacturaFlujo _facturaFlujo;

        public VentasController(IFacturaFlujo facturaFlujo)
        {
            _facturaFlujo = facturaFlujo;
        }

        // Vista inicial (después la hacemos tipo POS)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ✅ Acción POST para facturar un pedido ya creado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Facturar(int pedidoVentaId)
        {
            if (pedidoVentaId <= 0)
            {
                TempData["Error"] = "PedidoVentaId inválido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var facturaId = await _facturaFlujo.CrearDesdePedidoAsync(pedidoVentaId);
                TempData["Ok"] = $"Factura generada correctamente (# {facturaId}).";

                // Siguiente paso: vista de imprimir
                return RedirectToAction(nameof(Imprimir), new { facturaId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // Placeholder: luego hacemos la vista bonita tipo ticket
        [HttpGet]
        public IActionResult Imprimir(int facturaId)
        {
            ViewBag.FacturaId = facturaId;
            return View();
        }
    }
}