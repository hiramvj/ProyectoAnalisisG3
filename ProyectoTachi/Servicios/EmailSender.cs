using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ProyectoTachi.Servicios
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = ObtenerConfiguracionRequerida("Smtp:Host");
            var fromAddress = ObtenerConfiguracionRequerida("Smtp:FromAddress");
            var fromName = _configuration["Smtp:FromName"] ?? "Tachi Distribuidora";
            var username = _configuration["Smtp:Username"];
            var password = _configuration["Smtp:Password"];
            var port = _configuration.GetValue("Smtp:Port", 587);
            var enableSsl = _configuration.GetValue("Smtp:EnableSsl", true);

            using var message = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(email));

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            if (!string.IsNullOrWhiteSpace(username))
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException("Falta configurar Smtp:Password.");
                }

                smtpClient.Credentials = new NetworkCredential(username, password);
            }

            try
            {
                await smtpClient.SendMailAsync(message);
                _logger.LogInformation("Correo de recuperación enviado a {Email}.", email);
            }
            catch (SmtpException exception)
            {
                _logger.LogError(exception, "El servidor SMTP rechazó el correo destinado a {Email}.", email);
                throw;
            }
        }

        private string ObtenerConfiguracionRequerida(string clave)
        {
            var valor = _configuration[clave];
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new InvalidOperationException($"Falta configurar {clave} para el envío de correo.");
            }

            return valor;
        }
    }
}
