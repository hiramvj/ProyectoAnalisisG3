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
            var lista = await _flujo.ObtenerTodosAsync(true);
            return View(lista);
        }

        public async Task<IActionResult> Inactivos()
        {
            var lista = await _flujo.ObtenerTodosAsync(false);
            return View(lista);
        }

        public IActionResult Create()
        {
            return View(new TransportistaDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransportistaDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _flujo.AgregarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _flujo.ObtenerPorIdAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TransportistaDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _flujo.EditarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _flujo.CambiarEstadoAsync(id, false);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activar(int id)
        {
            await _flujo.CambiarEstadoAsync(id, true);
            return RedirectToAction(nameof(Inactivos));
        }
    }
}