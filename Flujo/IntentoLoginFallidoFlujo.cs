using Abstracciones.Interfaces.DA;
using Abstracciones.Modelos;

namespace Flujo
{
    public class IntentoLoginFallidoFlujo
    {
        private readonly IIntentoLoginFallidoDA _da;

        public IntentoLoginFallidoFlujo(IIntentoLoginFallidoDA da)
        {
            _da = da;
        }

        public async Task RegistrarAsync(string email, string ip, string userAgent, string motivo)
        {
            var intento = new IntentoLoginFallidoDto
            {
                EmailIngresado = email,
                FechaIntento = DateTime.UtcNow,
                IpAddress = ip,
                UserAgent = userAgent,
                Motivo = motivo
            };

            await _da.RegistrarAsync(intento);
        }

        public async Task<IEnumerable<IntentoLoginFallidoDto>> ObtenerAsync()
        {
            return await _da.ObtenerTodosAsync();
        }
    }
}