using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProveedoresController : Controller
    {
        private readonly IProveedorFlujo _flujo;

        public ProveedoresController(IProveedorFlujo flujo)
        {
            _flujo = flujo;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lista = await _flujo.ObtenerTodosAsync(true);
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al cargar los proveedores: {ex.Message}";
                return View(new List<ProveedorDto>());
            }
        }

        public async Task<IActionResult> Inactivos()
        {
            try
            {
                var lista = await _flujo.ObtenerTodosAsync(false);
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al cargar los proveedores inactivos: {ex.Message}";
                return View(new List<ProveedorDto>());
            }
        }

        public IActionResult Create()
        {
            return View(new ProveedorDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.AgregarAsync(dto);
                TempData["Ok"] = "Proveedor creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al guardar el proveedor: {ex.Message}");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _flujo.ObtenerPorIdAsync(id);
                if (dto == null) return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error al cargar el proveedor: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProveedorDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.EditarAsync(dto);
                TempData["Ok"] = "Proveedor actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al actualizar el proveedor: {ex.Message}");
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            try
            {
                await _flujo.CambiarEstadoAsync(id, false);
                TempData["Ok"] = "Proveedor desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo desactivar el proveedor: {ex.Message}";
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
                TempData["Ok"] = "Proveedor activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo activar el proveedor: {ex.Message}";
            }

            return RedirectToAction(nameof(Inactivos));
        }
    }
}