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
    public class TransportistaFlujo : ITransportistaFlujo
    {
        private readonly ITransportistaDA _da;

        public TransportistaFlujo(ITransportistaDA da)
        {
            _da = da;
        }

        public async Task<List<TransportistaDto>> ObtenerTodosAsync(bool activo)
        {
            return await _da.ObtenerTodosAsync(activo);
        }

        public async Task<TransportistaDto?> ObtenerPorIdAsync(int id)
        {
            return await _da.ObtenerPorIdAsync(id);
        }

        public async Task AgregarAsync(TransportistaDto dto)
        {
            await _da.AgregarAsync(dto);
        }

        public async Task EditarAsync(TransportistaDto dto)
        {
            await _da.EditarAsync(dto);
        }

        public async Task CambiarEstadoAsync(int id, bool activo)
        {
            await _da.CambiarEstadoAsync(id, activo);
        }
    }
}