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
    public class RutaEntregaFlujo : IRutaEntregaFlujo
    {
        private readonly IRutaEntregaDA _da;

        public RutaEntregaFlujo(IRutaEntregaDA da)
        {
            _da = da;
        }

        public async Task<List<RutaEntregaDto>> ObtenerTodasAsync()
        {
            return await _da.ObtenerTodasAsync();
        }

        public async Task<RutaEntregaDto?> ObtenerPorIdAsync(int id)
        {
            return await _da.ObtenerPorIdAsync(id);
        }

        public async Task AgregarAsync(RutaEntregaDto dto)
        {
            await _da.AgregarAsync(dto);
        }

        public async Task EditarAsync(RutaEntregaDto dto)
        {
            await _da.EditarAsync(dto);
        }

        public async Task CambiarEstadoAsync(int id, string estado)
        {
            await _da.CambiarEstadoAsync(id, estado);
        }
    }
}