using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProyectoTachi.Controllers
{
    [Authorize(Roles = "Admin,Ventas")]
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
            try
            {
                var lista = await _devolucionFlujo.ObtenerTodosAsync();
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar devoluciones: " + ex.Message;
                return View(new List<DevolucionListadoDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int? facturaId)
        {
            try
            {
                await CargarFacturasAsync(facturaId);

                if (facturaId.HasValue && facturaId.Value > 0)
                {
                    var lineas = await _devolucionFlujo.ObtenerLineasFacturaAsync(facturaId.Value);
                    ViewBag.Lineas = lineas;
                    ViewBag.FacturaIdSeleccionada = facturaId.Value;
                }

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la pantalla: " + ex.Message;
                await CargarFacturasAsync();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int facturaId, string motivo, List<DevolucionLineaDto> lineas)
        {
            try
            {
                lineas = lineas
    .Where(x => x.CantidadDevuelta > 0)
    .ToList();

                if (!lineas.Any())
                    throw new Exception("Debe ingresar al menos una cantidad mayor a 0.");

                if (facturaId <= 0)
                    throw new Exception("Debe seleccionar una factura.");

                if (!lineas.Any())
                    throw new Exception("Debe ingresar al menos una cantidad mayor a 0 para devolver.");

                if (string.IsNullOrWhiteSpace(motivo))
                    throw new Exception("Debe ingresar un motivo de devolución.");

                var devolucionId = await _devolucionFlujo.ProcesarDevolucionAsync(facturaId, lineas, motivo);

                TempData["Ok"] = "Devolución procesada exitosamente. Se generó la nota de crédito.";
                return RedirectToAction(nameof(Detalle), new { id = devolucionId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message
                    ?? ex.GetBaseException().Message
                    ?? ex.Message;

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
            try
            {
                var dto = await _devolucionFlujo.ObtenerPorIdAsync(id);
                if (dto == null) return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar detalle: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task CargarFacturasAsync(int? seleccionada = null)
        {
            try
            {
                var facturas = await _db.Facturas
                    .AsNoTracking()
                    .Where(f => f.Estado == "EMITIDA" || f.Estado == "DEVOLUCION_PARCIAL")
                    .OrderByDescending(f => f.NumeroFactura)
                    .Select(f => new
                    {
                        f.FacturaId,
                        Display = "Factura #" + f.NumeroFactura
                    })
                    .ToListAsync();

                ViewBag.Facturas = new SelectList(facturas, "FacturaId", "Display", seleccionada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error cargando facturas: " + ex.Message;
                ViewBag.Facturas = new SelectList(new List<object>(), "FacturaId", "Display");
            }
        }
    }
}
