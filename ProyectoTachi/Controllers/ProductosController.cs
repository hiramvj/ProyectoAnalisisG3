using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoFlujo _flujo;

        public ProductosController(IProductoFlujo flujo)
        {
            _flujo = flujo;
        }

        // LISTA ACTIVOS
        public async Task<IActionResult> Index()
        {
            var lista = await _flujo.ObtenerTodosAsync(true);
            return View(lista);
        }

        // LISTA INACTIVOS
        [HttpGet]
        public async Task<IActionResult> Inactivos()
        {
            var productos = await _flujo.ObtenerTodosAsync(false);
            return View(productos);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Abstracciones.Modelos.ProductoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Abstracciones.Modelos.ProductoDto dto)
        {
            // Validaciones mínimas (después refinamos)
            if (string.IsNullOrWhiteSpace(dto.SKU))
                ModelState.AddModelError(nameof(dto.SKU), "SKU es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                ModelState.AddModelError(nameof(dto.Nombre), "Nombre es requerido.");

            if (dto.UnidadMedidaId <= 0)
                ModelState.AddModelError(nameof(dto.UnidadMedidaId), "Unidad de medida es requerida.");

            if (dto.Costo < 0)
                ModelState.AddModelError(nameof(dto.Costo), "Costo no puede ser negativo.");

            if (dto.Precio < 0)
                ModelState.AddModelError(nameof(dto.Precio), "Precio no puede ser negativo.");

            if (!ModelState.IsValid)
                return View(dto);

            await _flujo.AgregarAsync(dto);

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _flujo.ObtenerPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // EDIT (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(ProductoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _flujo.EditarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // DESACTIVAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _flujo.CambiarEstadoAsync(id, false);
            return RedirectToAction(nameof(Index));
        }

        // ACTIVAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            await _flujo.CambiarEstadoAsync(id, true);
            return RedirectToAction(nameof(Inactivos));
        }
    }
}
