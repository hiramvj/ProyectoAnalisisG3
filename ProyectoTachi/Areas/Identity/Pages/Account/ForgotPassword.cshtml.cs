#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace ProyectoTachi.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool SolicitudProcesada { get; private set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es requerido.")]
            [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; }
        }

        public void OnGet(bool enviado = false)
        {
            SolicitudProcesada = enviado;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToPage("./ForgotPassword", new { enviado = true });
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            try
            {
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Restablecer contraseña - Tachi Distribuidora",
                    $"Para restablecer su contraseña, haga clic en el siguiente enlace: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Restablecer contraseña</a>");
            }
            catch (SmtpException)
            {
                ModelState.AddModelError(string.Empty,
                    "No fue posible enviar el correo. Verifica la configuración de la cuenta remitente e inténtalo nuevamente.");
                return Page();
            }

            return RedirectToPage("./ForgotPassword", new { enviado = true });
        }
    }
}
