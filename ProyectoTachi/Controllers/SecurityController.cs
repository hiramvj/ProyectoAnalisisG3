using Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly IntentoLoginFallidoFlujo _flujo;

        public SecurityController(IntentoLoginFallidoFlujo flujo)
        {
            _flujo = flujo;
        }

        public async Task<IActionResult> Intentos()
        {
            var intentos = await _flujo.ObtenerAsync();
            return View(intentos);
        }
    }
}