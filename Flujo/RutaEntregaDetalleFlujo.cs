using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flujo
{
    public class RutaEntregaDetalleFlujo : IRutaEntregaDetalleFlujo
    {
        private readonly IRutaEntregaDetalleDA _da;

        public RutaEntregaDetalleFlujo(IRutaEntregaDetalleDA da)
        {
            _da = da;
        }

        public async Task<List<RutaEntregaDetalleVistaDto>> ObtenerPorRutaAsync(int rutaId)
        {
            return await _da.ObtenerPorRutaAsync(rutaId);
        }

        public async Task AgregarAsync(RutaEntregaDetalleDto dto)
        {
            await _da.AgregarAsync(dto);
        }

        public async Task CambiarEstadoParadaAsync(int rutaDetalleId, string estadoParada)
        {
            await _da.CambiarEstadoParadaAsync(rutaDetalleId, estadoParada);
        }

        public async Task EliminarAsync(int rutaDetalleId)
        {
            await _da.EliminarAsync(rutaDetalleId);
        }
    }
}