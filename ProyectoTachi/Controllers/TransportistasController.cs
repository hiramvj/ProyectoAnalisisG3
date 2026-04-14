using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class TransportistasController : Controller
    {
        private readonly ITransportistaFlujo _flujo;

        public TransportistasController(ITransportistaFlujo flujo)
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
                TempData["MensajeError"] = $"Error al cargar transportistas: {ex.Message}";
                return View(new List<TransportistaDto>());
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
                TempData["MensajeError"] = $"Error al cargar transportistas inactivos: {ex.Message}";
                return View(new List<TransportistaDto>());
            }
        }

        public IActionResult Create()
        {
            return View(new TransportistaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransportistaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.AgregarAsync(dto);
                TempData["MensajeExito"] = "Transportista creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al guardar transportista: {ex.Message}");
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
                TempData["MensajeError"] = $"Error al cargar transportista: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransportistaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.EditarAsync(dto);
                TempData["MensajeExito"] = "Transportista actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar transportista: {ex.Message}");
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
                TempData["MensajeExito"] = "Transportista desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al desactivar: {ex.Message}";
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
                TempData["MensajeExito"] = "Transportista activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al activar: {ex.Message}";
            }

            return RedirectToAction(nameof(Inactivos));
        }
    }
}