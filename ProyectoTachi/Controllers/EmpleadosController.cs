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
            try
            {
                var lista = await _flujo.ObtenerTodosAsync(true);
                return View(lista);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar los empleados: {ex.Message}";
                return View(new List<EmpleadoDto>());
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
                TempData["MensajeError"] = $"Ocurrió un error al cargar los empleados inactivos: {ex.Message}";
                return View(new List<EmpleadoDto>());
            }
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
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.AgregarAsync(dto);
                TempData["MensajeExito"] = "Empleado registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al guardar el empleado: {ex.Message}");
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _flujo.ObtenerPorIdAsync(id);
                if (dto == null)
                    return NotFound();

                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar el empleado: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpleadoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.EditarAsync(dto);
                TempData["MensajeExito"] = "Empleado actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al actualizar el empleado: {ex.Message}");
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
                TempData["MensajeExito"] = "Empleado desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"No se pudo desactivar el empleado: {ex.Message}";
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
                TempData["MensajeExito"] = "Empleado activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"No se pudo activar el empleado: {ex.Message}";
            }

            return RedirectToAction(nameof(Inactivos));
        }

        public async Task<IActionResult> Asistencias(int id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar las asistencias: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
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

            try
            {
                await _asistenciaFlujo.AgregarAsync(dto);
                TempData["MensajeExito"] = "Asistencia registrada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al registrar la asistencia: {ex.Message}";
            }

            return RedirectToAction(nameof(Asistencias), new { id = dto.EmpleadoId });
        }
    }
}