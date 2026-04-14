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
    public class ComprasController : Controller
    {
        private readonly IOrdenCompraFlujo _ordenFlujo;
        private readonly AppDbContext _db;

        public ComprasController(IOrdenCompraFlujo ordenFlujo, AppDbContext db)
        {
            _ordenFlujo = ordenFlujo;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                await CargarProveedoresAsync();
                await CargarProductosAsync();
                return View(new OrdenCompraCrearDto());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al cargar la pantalla de compras: {ex.Message}";
                return View(new OrdenCompraCrearDto());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearOrden(OrdenCompraCrearDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarProveedoresAsync(dto.ProveedorId);
                    await CargarProductosAsync();
                    return View("Index", dto);
                }

                var ordenId = await _ordenFlujo.CrearOrdenAsync(dto);
                TempData["Ok"] = $"Orden de compra creada exitosamente con ID: {ordenId}";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;

                await CargarProveedoresAsync(dto.ProveedorId);
                await CargarProductosAsync();
                return View("Index", dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message
                    ?? ex.GetBaseException().Message
                    ?? ex.Message;

                await CargarProveedoresAsync(dto.ProveedorId);
                await CargarProductosAsync();
                return View("Index", dto);
            }
        }

        private async Task CargarProveedoresAsync(int? seleccionado = null)
        {
            var proveedores = await _db.Proveedores.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.NombreLegal)
                .ToListAsync();

            ViewBag.Proveedores = new SelectList(proveedores, "ProveedorId", "NombreLegal", seleccionado);
        }

        private async Task CargarProductosAsync()
        {
            var productos = await _db.Productos.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            ViewBag.Productos = productos;
        }
    }
}