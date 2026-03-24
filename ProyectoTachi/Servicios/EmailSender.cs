using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace ProyectoTachi.Servicios
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning("========== EMAIL ==========");
            _logger.LogWarning("Para: {Email}", email);
            _logger.LogWarning("Asunto: {Subject}", subject);
            _logger.LogWarning("Contenido: {Message}", htmlMessage);
            _logger.LogWarning("===========================");
            return Task.CompletedTask;
        }
    }
}
