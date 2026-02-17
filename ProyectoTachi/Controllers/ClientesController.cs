using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteFlujo _flujo;

        public ClientesController(IClienteFlujo flujo)
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
            var clientes = await _flujo.ObtenerTodosAsync(false);
            return View(clientes);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteDto());
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                ModelState.AddModelError(nameof(dto.NombreCompleto), "El nombre completo es requerido.");

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
            if (dto == null) return NotFound();
            return View(dto);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _flujo.EditarAsync(dto);

            if (!ok)
            {
                ModelState.AddModelError("", "No se pudo actualizar el cliente.");
                return View(dto);
            }

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
