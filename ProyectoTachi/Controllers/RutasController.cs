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
    public class RutasController : Controller
    {
        private readonly IRutaEntregaFlujo _rutaFlujo;
        private readonly IRutaEntregaDetalleFlujo _detalleFlujo;
        private readonly ITransportistaFlujo _transportistaFlujo;
        private readonly AppDbContext _db;

        public RutasController(
            IRutaEntregaFlujo rutaFlujo,
            IRutaEntregaDetalleFlujo detalleFlujo,
            ITransportistaFlujo transportistaFlujo,
            AppDbContext db)
        {
            _rutaFlujo = rutaFlujo;
            _detalleFlujo = detalleFlujo;
            _transportistaFlujo = transportistaFlujo;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _rutaFlujo.ObtenerTodasAsync();
            return View(lista);
        }

        public async Task<IActionResult> Create()
        {
            await CargarTransportistasAsync();
            return View(new RutaEntregaDto
            {
                Estado = "PLANIFICADA",
                FechaProgramada = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RutaEntregaDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarTransportistasAsync(dto.TransportistaId);
                return View(dto);
            }

            await _rutaFlujo.AgregarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _rutaFlujo.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            await CargarTransportistasAsync(dto.TransportistaId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RutaEntregaDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarTransportistasAsync(dto.TransportistaId);
                return View(dto);
            }

            await _rutaFlujo.EditarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var ruta = await _rutaFlujo.ObtenerPorIdAsync(id);
            if (ruta == null) return NotFound();

            var detalles = await _detalleFlujo.ObtenerPorRutaAsync(id);

            ViewBag.Ruta = ruta;
            ViewBag.Pedidos = await ObtenerPedidosDisponiblesAsync();
            ViewBag.NuevaParada = new RutaEntregaDetalleDto
            {
                RutaId = id,
                EstadoParada = "PENDIENTE",
                OrdenParada = detalles.Count + 1
            };

            return View(detalles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPedido(RutaEntregaDetalleDto dto)
        {
            if (!ModelState.IsValid)
            {
                var ruta = await _rutaFlujo.ObtenerPorIdAsync(dto.RutaId);
                var detalles = await _detalleFlujo.ObtenerPorRutaAsync(dto.RutaId);

                ViewBag.Ruta = ruta;
                ViewBag.Pedidos = await ObtenerPedidosDisponiblesAsync();
                ViewBag.NuevaParada = dto;

                return View("Detalle", detalles);
            }

            await _detalleFlujo.AgregarAsync(dto);
            return RedirectToAction(nameof(Detalle), new { id = dto.RutaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoParada(int rutaDetalleId, string estadoParada, int rutaId)
        {
            await _detalleFlujo.CambiarEstadoParadaAsync(rutaDetalleId, estadoParada);
            return RedirectToAction(nameof(Detalle), new { id = rutaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPedido(int rutaDetalleId, int rutaId)
        {
            await _detalleFlujo.EliminarAsync(rutaDetalleId);
            return RedirectToAction(nameof(Detalle), new { id = rutaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstadoRuta(int rutaId, string estado)
        {
            await _rutaFlujo.CambiarEstadoAsync(rutaId, estado);
            return RedirectToAction(nameof(Detalle), new { id = rutaId });
        }

        private async Task CargarTransportistasAsync(int? seleccionado = null)
        {
            var transportistas = await _transportistaFlujo.ObtenerTodosAsync(true);

            ViewBag.Transportistas = new SelectList(
                transportistas.OrderBy(t => t.NombreCompleto),
                "TransportistaId",
                "NombreCompleto",
                seleccionado
            );
        }

        private async Task<List<SelectListItem>> ObtenerPedidosDisponiblesAsync()
        {
            var pedidos = await _db.PedidoVentas
                .AsNoTracking()
                .OrderByDescending(p => p.PedidoVentaId)
                .Select(p => new SelectListItem
                {
                    Value = p.PedidoVentaId.ToString(),
                    Text = "Pedido #" + p.PedidoVentaId
                })
                .ToListAsync();

            return pedidos;
        }
    }
}