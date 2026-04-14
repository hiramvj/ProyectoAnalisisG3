using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class CuentasPorPagarController : Controller
    {
        private readonly ICuentasPorPagarFlujo _cuentasFlujo;
        private readonly IProveedorFlujo _proveedorFlujo;

        public CuentasPorPagarController(ICuentasPorPagarFlujo cuentasFlujo, IProveedorFlujo proveedorFlujo)
        {
            _cuentasFlujo = cuentasFlujo;
            _proveedorFlujo = proveedorFlujo;
        }

        // Listado de cuentas
        public async Task<IActionResult> Index()
        {
            var cuentas = await _cuentasFlujo.ListarCuentasAsync();
            return View(cuentas);
        }

        // Formulario para crear una nueva factura/cuenta
        [HttpGet]
        public async Task<IActionResult> CrearFactura()
        {
            await CargarProveedoresAsync();
            return View(new CuentaPorPagarDto { FechaEmision = DateTime.Today, FechaVencimiento = DateTime.Today.AddDays(30) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearFactura(CuentaPorPagarDto dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarProveedoresAsync();
                return View(dto);
            }

            try
            {
                var id = await _cuentasFlujo.CrearCuentaAsync(dto);
                TempData["ContabilidadExito"] = $"Cuenta registrada correctamente con el ID #{id}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await CargarProveedoresAsync();
                return View(dto);
            }
        }

        private async Task CargarProveedoresAsync()
        {
            var proveedores = await _proveedorFlujo.ObtenerTodosAsync(true);
            ViewBag.Proveedores = proveedores.OrderBy(p => p.NombreLegal).ToList();
        }

        // Detalle de la cuenta y listado de sus pagos

        public async Task<IActionResult> Detalle(int id)
        {
            var cuenta = await _cuentasFlujo.ObtenerCuentaAsync(id);
            if (cuenta == null) return NotFound();

            var pagos = await _cuentasFlujo.ListarPagosPorCuentaAsync(id);
            ViewData["Pagos"] = pagos;

            return View(cuenta);
        }

        // Formulario para registrar un pago o anticipo
        [HttpGet]
        public async Task<IActionResult> RegistrarPago(int id)
        {
            var cuenta = await _cuentasFlujo.ObtenerCuentaAsync(id);
            if (cuenta == null) return NotFound();

            if (cuenta.SaldoPendiente <= 0)
            {
                TempData["ContabilidadInfo"] = "Esta cuenta ya está pagada en su totalidad.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var pago = new PagoProveedorDto
            {
                CuentaPorPagarId = id,
                FechaPago = DateTime.Today,
                Monto = cuenta.SaldoPendiente, // Por defecto ofrece pagar todo el saldo
                TipoTransaccion = "PAGO",
                Estado = "COMPLETADO"
            };

            ViewData["Cuenta"] = cuenta;
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPago(PagoProveedorDto dto)
        {
            if (!ModelState.IsValid)
            {
                var cuentaParams = await _cuentasFlujo.ObtenerCuentaAsync(dto.CuentaPorPagarId);
                ViewData["Cuenta"] = cuentaParams;
                return View(dto);
            }

            try
            {
                await _cuentasFlujo.RegistrarPagoAsync(dto);
                TempData["ContabilidadExito"] = "Transacción registrada correctamente.";
                return RedirectToAction(nameof(Detalle), new { id = dto.CuentaPorPagarId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var cuenta = await _cuentasFlujo.ObtenerCuentaAsync(dto.CuentaPorPagarId);
                ViewData["Cuenta"] = cuenta;
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarPagoProgramado(int id, int idCuenta)
        {
            try
            {
                await _cuentasFlujo.CompletarPagoProgramadoAsync(id);
                TempData["ContabilidadExito"] = "El pago programado ha sido marcado como completado.";
            }
            catch (Exception ex)
            {
                TempData["ContabilidadError"] = ex.Message;
            }

            return RedirectToAction(nameof(Detalle), new { id = idCuenta });
        }
    }
}
