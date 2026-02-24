using Abstracciones.Interfaces.Flujo;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    public class FacturasController : Controller
    {
        private readonly IFacturaFlujo _flujo;

        public FacturasController(IFacturaFlujo flujo)
        {
            _flujo = flujo;
        }

        public async Task<IActionResult> Index(string? estado, DateTime? fechaEmision)
        {
            var lista = await _flujo.ListarAsync(estado, fechaEmision);
            return View(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Enviar(int id)
        {
            await _flujo.CambiarEstadoAsync(id, "Pendiente Respuesta");
            return RedirectToAction(nameof(Index));
        }
    }
}