using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoTachi.Servicios;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private readonly IPedidoVentaFlujo _pedidoFlujo;
        private readonly IFacturaFlujo _facturaFlujo;
        private readonly AppDbContext _db;

        public VentasController(IPedidoVentaFlujo pedidoFlujo, IFacturaFlujo facturaFlujo, AppDbContext db)
        {
            _pedidoFlujo = pedidoFlujo;
            _facturaFlujo = facturaFlujo;
            _db = db;
        }
        [HttpGet]
        public async Task<IActionResult> Historial(string? q, DateTime? desde, DateTime? hasta, int? clienteId, string? estado, int? metodoPagoId)
        {
            await CargarClientesAsync(clienteId);
            await CargarMetodosPagoAsync(metodoPagoId);

            ViewBag.Q = q;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            ViewBag.Estado = estado;
            ViewBag.MetodoPagoId = metodoPagoId;

            var lista = await _pedidoFlujo.ListarAsync(q, desde, hasta, clienteId, estado, metodoPagoId);
            return View(lista); // ✅ va a Views/Ventas/Historial.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPedido(PedidoVentaCrearDto dto)
        {
            try
            {
                var pedidoId = await _pedidoFlujo.CrearPedidoAsync(dto);
                TempData["Ok"] = $"Pedido creado: {pedidoId}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await CargarClientesAsync(dto.ClienteId);
                await CargarProductosAsync();
                await CargarMetodosPagoAsync(dto.MetodoPagoId); // ✅
                return View("Index", dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await CargarClientesAsync();
            await CargarProductosAsync();
            await CargarMetodosPagoAsync(); 
            return View(new PedidoVentaCrearDto());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Facturar(int pedidoVentaId)
        {
            try
            {
                var facturaId = await _facturaFlujo.CrearDesdePedidoAsync(pedidoVentaId);
                TempData["Ok"] = $"Factura generada: {facturaId}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message
                    ?? ex.GetBaseException().Message
                    ?? ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var dto = await _pedidoFlujo.ObtenerDetalleAsync(id);
            if (dto == null) return NotFound();
            return View(dto); // Views/Ventas/Detalle.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var det = await _pedidoFlujo.ObtenerDetalleAsync(id);
            if (det == null) return NotFound();

            await CargarMetodosPagoAsync(det.MetodoPagoId);

            var dto = new PedidoVentaEditarDto
            {
                PedidoVentaId = det.PedidoVentaId,
                Estado = det.Estado,
                MetodoPagoId = det.MetodoPagoId,
                Observaciones = det.Observaciones
            };

            return View(dto); // Views/Ventas/Editar.cshtml
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PedidoVentaEditarDto dto)
        {
            if (dto.PedidoVentaId <= 0)
                ModelState.AddModelError("", "Pedido inválido.");

            if (string.IsNullOrWhiteSpace(dto.Estado))
                ModelState.AddModelError(nameof(dto.Estado), "Estado requerido.");

            if (!ModelState.IsValid)
            {
                await CargarMetodosPagoAsync(dto.MetodoPagoId);
                return View(dto);
            }

            var ok = await _pedidoFlujo.EditarEncabezadoAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo actualizar el pedido.");
                await CargarMetodosPagoAsync(dto.MetodoPagoId);
                return View(dto);
            }

            return RedirectToAction(nameof(Historial));
        }

        private async Task CargarClientesAsync(int? seleccionado = null)
        {
            var clientes = await _db.Clientes.AsNoTracking()
                .OrderBy(c => c.NombreCompleto)
                .ToListAsync();

            ViewBag.Clientes = new SelectList(clientes, "ClienteId", "NombreCompleto", seleccionado);
        }

        private async Task CargarProductosAsync()
        {
            var productos = await _db.Productos.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            ViewBag.Productos = productos; 
        }
        private async Task CargarMetodosPagoAsync(int? seleccionado = null)
        {
            var metodos = await _db.MetodosPago.AsNoTracking()
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            ViewBag.MetodosPago = new SelectList(metodos, "MetodoPagoId", "Nombre", seleccionado);
        }
        public async Task<IActionResult> ProductosMasVendidos()
        {
            var productos = await _pedidoFlujo.ObtenerProductosMasVendidosAsync();
            return View(productos);
        }
        public async Task<IActionResult> ImprimirFactura(int id)
        {
            var dto = await _pedidoFlujo.ObtenerDetalleAsync(id);
            if (dto == null) return NotFound();

            QuestPDF.Settings.License = LicenseType.Community;

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "tachi-logo.png");

            var document = new FacturaPdfDocument(dto, logoPath);
            var pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Factura_{dto.NumeroPedido}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> NotaAjuste(int id)
        {
            var detalle = await _pedidoFlujo.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            
            var model = new NotaContableDto
            {
                FacturaId = id,
                MontoMaximo = detalle.Total 
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NotaAjuste(NotaContableDto dto)
        {
            try
            {
                await _facturaFlujo.AplicarNotaAjusteAsync(dto);
                return Json(new { success = true, message = "¡Ajuste aplicado correctamente!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}