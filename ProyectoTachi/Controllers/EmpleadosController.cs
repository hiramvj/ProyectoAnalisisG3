using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private readonly IEmpleadoFlujo _flujo;
        private readonly IAsistenciaFlujo _asistenciaFlujo;

        public EmpleadosController(IEmpleadoFlujo flujo, IAsistenciaFlujo asistenciaFlujo)
        {
            _flujo = flujo;
            _asistenciaFlujo = asistenciaFlujo;
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
            return View(new EmpleadoDto
            {
                Activo = true,
                FechaIngreso = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _flujo.AgregarAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _flujo.ObtenerPorIdAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpleadoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

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

        public async Task<IActionResult> Asistencias(int id)
        {
            var empleado = await _flujo.ObtenerPorIdAsync(id);
            if (empleado == null)
                return NotFound();

            var asistencias = await _asistenciaFlujo.ObtenerPorEmpleadoAsync(id);

            ViewBag.Empleado = empleado;
            ViewBag.NuevaAsistencia = new AsistenciaDto
            {
                EmpleadoId = id,
                Fecha = DateTime.Today,
                Tipo = "ASISTENCIA"
            };

            return View(asistencias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAsistencia(AsistenciaDto dto)
        {
            if (!ModelState.IsValid)
            {
                var empleado = await _flujo.ObtenerPorIdAsync(dto.EmpleadoId);
                var asistencias = await _asistenciaFlujo.ObtenerPorEmpleadoAsync(dto.EmpleadoId);

                ViewBag.Empleado = empleado;
                ViewBag.NuevaAsistencia = dto;

                return View("Asistencias", asistencias);
            }

            if (dto.Tipo != "ASISTENCIA")
            {
                dto.HoraEntrada = null;
                dto.HoraSalida = null;
            }

            await _asistenciaFlujo.AgregarAsync(dto);
            return RedirectToAction(nameof(Asistencias), new { id = dto.EmpleadoId });
        }
    }
}