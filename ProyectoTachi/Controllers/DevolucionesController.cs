using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class DevolucionesController : Controller
    {
        private readonly IDevolucionVentaFlujo _devolucionFlujo;
        private readonly AppDbContext _db;

        public DevolucionesController(IDevolucionVentaFlujo devolucionFlujo, AppDbContext db)
        {
            _devolucionFlujo = devolucionFlujo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lista = await _devolucionFlujo.ObtenerTodosAsync();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int? facturaId)
        {
            await CargarFacturasAsync(facturaId);

            if (facturaId.HasValue && facturaId.Value > 0)
            {
                try
                {
                    var lineas = await _devolucionFlujo.ObtenerLineasFacturaAsync(facturaId.Value);
                    ViewBag.Lineas = lineas;
                    ViewBag.FacturaIdSeleccionada = facturaId.Value;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int facturaId, string motivo, List<DevolucionLineaDto> lineas)
        {
            try
            {
                var devolucionId = await _devolucionFlujo.ProcesarDevolucionAsync(facturaId, lineas, motivo);
                TempData["Ok"] = "Devolución procesada exitosamente. Se generó la nota de crédito.";
                return RedirectToAction(nameof(Detalle), new { id = devolucionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await CargarFacturasAsync(facturaId);

                try
                {
                    var lineasFactura = await _devolucionFlujo.ObtenerLineasFacturaAsync(facturaId);
                    ViewBag.Lineas = lineasFactura;
                    ViewBag.FacturaIdSeleccionada = facturaId;
                }
                catch { }

                ViewBag.Motivo = motivo;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var dto = await _devolucionFlujo.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        private async Task CargarFacturasAsync(int? seleccionada = null)
        {
            var facturas = await _db.Facturas
                .AsNoTracking()
                .Where(f => f.Estado == "EMITIDA" || f.Estado == "DEVOLUCION_PARCIAL")
                .OrderByDescending(f => f.NumeroFactura)
                .Select(f => new { f.FacturaId, Display = "Factura #" + f.NumeroFactura })
                .ToListAsync();

            ViewBag.Facturas = new SelectList(facturas, "FacturaId", "Display", seleccionada);
        }
    }
}
