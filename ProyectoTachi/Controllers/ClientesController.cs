using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteFlujo _flujo;
        private readonly IPedidoVentaFlujo _pedidoFlujo;

        public ClientesController(IClienteFlujo flujo, IPedidoVentaFlujo pedidoFlujo)
        {
            _flujo = flujo;
            _pedidoFlujo = pedidoFlujo;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            try
            {
                int pageSize = 10;

                var lista = await _flujo.ObtenerTodosAsync(true);

                if (!string.IsNullOrWhiteSpace(q))
                {
                    q = q.Trim();

                    lista = lista
                        .Where(c =>
                            (!string.IsNullOrWhiteSpace(c.Identificacion) && c.Identificacion.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.NombreCompleto) && c.NombreCompleto.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(c.Correo) && c.Correo.Contains(q, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }

                lista = lista
                    .OrderByDescending(c => c.ClienteId)
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

                ViewBag.TotalClientesActivos = totalRegistros;
                ViewBag.Q = q;
                ViewBag.PaginaActual = page;
                ViewBag.TotalPaginas = totalPaginas;

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar los clientes: {ex.Message}";
                return View(new List<ClienteDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Inactivos()
        {
            try
            {
                var clientes = await _flujo.ObtenerTodosAsync(false);
                return View(clientes);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar los clientes inactivos: {ex.Message}";
                return View(new List<ClienteDto>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ClienteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteDto dto)
        {
            try
            {
                await ValidarClienteAsync(dto);

                if (!ModelState.IsValid)
                    return View(dto);

                await _flujo.AgregarAsync(dto);

                TempData["MensajeExito"] = "Cliente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al guardar el cliente: {ex.Message}");
                return View(dto);
            }
        }

        [HttpGet]
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
                TempData["MensajeError"] = $"Ocurrió un error al cargar el cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteDto dto)
        {
            try
            {
                await ValidarClienteAsync(dto);

                if (!ModelState.IsValid)
                    return View(dto);

                var ok = await _flujo.EditarAsync(dto);

                if (!ok)
                {
                    ModelState.AddModelError("", "No se pudo actualizar el cliente.");
                    return View(dto);
                }

                TempData["MensajeExito"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al actualizar el cliente: {ex.Message}");
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
                TempData["MensajeExito"] = "Cliente desactivado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"No se pudo desactivar el cliente: {ex.Message}";
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
                TempData["MensajeExito"] = "Cliente activado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"No se pudo activar el cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Inactivos));
        }


        private async Task ValidarClienteAsync(ClienteDto dto)
        {
            dto.NombreCompleto = dto.NombreCompleto?.Trim() ?? string.Empty;
            dto.Identificacion = dto.Identificacion?.Trim();
            dto.Correo = dto.Correo?.Trim();
            dto.Telefono = dto.Telefono?.Trim();
            dto.Direccion = dto.Direccion?.Trim();

            if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                ModelState.AddModelError(nameof(dto.NombreCompleto), "El nombre completo es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Identificacion))
                ModelState.AddModelError(nameof(dto.Identificacion), "La identificación es requerida.");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                ModelState.AddModelError(nameof(dto.Correo), "El correo es requerido.");

            if (!ModelState.IsValid)
                return;

            var clientes = new List<ClienteDto>();
            clientes.AddRange(await _flujo.ObtenerTodosAsync(true));
            clientes.AddRange(await _flujo.ObtenerTodosAsync(false));

            var duplicadoIdentificacion = clientes.Any(c =>
                c.ClienteId != dto.ClienteId &&
                !string.IsNullOrWhiteSpace(c.Identificacion) &&
                string.Equals(c.Identificacion.Trim(), dto.Identificacion, StringComparison.OrdinalIgnoreCase));

            if (duplicadoIdentificacion)
                ModelState.AddModelError(nameof(dto.Identificacion), "Ya existe un cliente con esta identificación.");

            var duplicadoCorreo = clientes.Any(c =>
                c.ClienteId != dto.ClienteId &&
                !string.IsNullOrWhiteSpace(c.Correo) &&
                string.Equals(c.Correo.Trim(), dto.Correo, StringComparison.OrdinalIgnoreCase));

            if (duplicadoCorreo)
                ModelState.AddModelError(nameof(dto.Correo), "Ya existe un cliente con este correo.");
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var cliente = await _flujo.ObtenerPorIdAsync(id);
                if (cliente == null)
                    return NotFound();

                var historial = await _pedidoFlujo.ObtenerHistorialClienteAsync(id);
                ViewBag.HistorialCompras = historial;

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Ocurrió un error al cargar el detalle del cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}