using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IRutaEntregaDetalleFlujo
    {
        Task<List<RutaEntregaDetalleVistaDto>> ObtenerPorRutaAsync(int rutaId);
        Task AgregarAsync(RutaEntregaDetalleDto dto);
        Task CambiarEstadoParadaAsync(int rutaDetalleId, string estadoParada);
        Task EliminarAsync(int rutaDetalleId);
    }
}
