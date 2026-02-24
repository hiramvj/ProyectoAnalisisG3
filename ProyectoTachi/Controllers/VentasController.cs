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
        public async Task<IActionResult> Index()
        {
            await CargarClientesAsync();
            await CargarProductosAsync();
            return View(new PedidoVentaCrearDto());
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
                return View("Index", dto);
            }
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
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
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

            ViewBag.Productos = productos; // lo usamos para armar líneas
        }
    }
}