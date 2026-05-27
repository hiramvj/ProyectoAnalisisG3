using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using DA.Contexto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ProyectoTachi.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoFlujo _flujo;
        private readonly AppDbContext _db;

        public ProductosController(IProductoFlujo flujo, AppDbContext db)
        {
            _flujo = flujo;
            _db = db;
        }

        public async Task<IActionResult> Index(string nombre, decimal? precioMin, decimal? precioMax, int page = 1)
        {
            try
            {
                int pageSize = 10;

                var lista = await _flujo.ObtenerTodosAsync(true);

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    lista = lista
                        .Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (precioMin.HasValue)
                {
                    lista = lista
                        .Where(p => p.Precio >= precioMin.Value)
                        .ToList();
                }

                if (precioMax.HasValue)
                {
                    lista = lista
                        .Where(p => p.Precio <= precioMax.Value)
                        .ToList();
                }

                lista = lista
                    .OrderByDescending(p => p.ProductoId)
                    .ToList();

                var totalRegistros = lista.Count;
                var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

                if (page < 1)
                {
                    page = 1;
                }

                if (totalPaginas > 0 && page > totalPaginas)
                {
                    page = totalPaginas;
                }

                var listaPaginada = lista
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.PaginaActual = page;
                ViewBag.TotalPaginas = totalPaginas;

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar productos: {ex.Message}";
                return View(new List<ProductoDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Inactivos()
        {
            try
            {
                var productos = await _flujo.ObtenerTodosAsync(false);
                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar productos inactivos: {ex.Message}";
                return View(new List<ProductoDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await CargarCategoriasAsync();
                await CargarUnidadesAsync();
                return View(new ProductoDto());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar formulario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDto dto)
        {
            try
            {
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
                {
                    await CargarCategoriasAsync(dto.CategoriaProductoId);
                    await CargarUnidadesAsync(dto.UnidadMedidaId);
                    return View(dto);
                }

                await _flujo.AgregarAsync(dto);

                TempData["Ok"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar producto: {ex.Message}");
            }

            await CargarCategoriasAsync(dto.CategoriaProductoId);
            await CargarUnidadesAsync(dto.UnidadMedidaId);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _flujo.ObtenerPorIdAsync(id);
                if (dto == null) return NotFound();

                await CargarCategoriasAsync(dto.CategoriaProductoId);
                await CargarUnidadesAsync(dto.UnidadMedidaId);

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar producto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await CargarCategoriasAsync(dto.CategoriaProductoId);
                    await CargarUnidadesAsync(dto.UnidadMedidaId);
                    return View(dto);
                }

                var ok = await _flujo.EditarAsync(dto);

                if (!ok)
                {
                    ModelState.AddModelError("", "No se pudo actualizar el producto.");
                }
                else
                {
                    TempData["Ok"] = "Producto actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar producto: {ex.Message}");
            }

            await CargarCategoriasAsync(dto.CategoriaProductoId);
            await CargarUnidadesAsync(dto.UnidadMedidaId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                await _flujo.CambiarEstadoAsync(id, false);
                TempData["Ok"] = "Producto desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al desactivar: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                await _flujo.CambiarEstadoAsync(id, true);
                TempData["Ok"] = "Producto activado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al activar: {ex.Message}";
            }

            return RedirectToAction(nameof(Inactivos));
        }

        private async Task CargarCategoriasAsync(int? seleccionada = null)
        {
            var cats = await _db.CategoriasProducto
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.Categorias = new SelectList(cats, "CategoriaProductoId", "Nombre", seleccionada);
        }

        private async Task CargarUnidadesAsync(int? seleccionada = null)
        {
            var unidades = await _db.UnidadesMedida
                .AsNoTracking()
                .OrderBy(u => u.Nombre)
                .ToListAsync();

            ViewBag.Unidades = new SelectList(unidades, "UnidadMedidaId", "Nombre", seleccionada);
        }
    }
}