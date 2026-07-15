using Flujo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoTachi.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly IntentoLoginFallidoFlujo _flujo;
        private readonly UserManager<IdentityUser> _userManager;

        public SecurityController(IntentoLoginFallidoFlujo flujo, UserManager<IdentityUser> userManager)
        {
            _flujo = flujo;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Intentos()
        {
            var intentos = await _flujo.ObtenerAsync();
            return View(intentos);
        }

        public async Task<IActionResult> Perfil()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return Challenge();
            }

            return View(usuario);
        }
    }
}
