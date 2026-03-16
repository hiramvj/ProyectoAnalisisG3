using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IRutaEntregaDetalleDA
    {
        Task<List<RutaEntregaDetalleVistaDto>> ObtenerPorRutaAsync(int rutaId);
        Task AgregarAsync(RutaEntregaDetalleDto entidad);
        Task CambiarEstadoParadaAsync(int rutaDetalleId, string estadoParada);
        Task EliminarAsync(int rutaDetalleId);
    }
}
