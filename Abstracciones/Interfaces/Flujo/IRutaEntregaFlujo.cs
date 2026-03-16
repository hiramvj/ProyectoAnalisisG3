using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IRutaEntregaFlujo
    {
        Task<List<RutaEntregaDto>> ObtenerTodasAsync();
        Task<RutaEntregaDto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(RutaEntregaDto dto);
        Task EditarAsync(RutaEntregaDto dto);
        Task CambiarEstadoAsync(int id, string estado);
    }
}
