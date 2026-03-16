using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IRutaEntregaDA
    {
        Task<List<RutaEntregaDto>> ObtenerTodasAsync();
        Task<RutaEntregaDto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(RutaEntregaDto entidad);
        Task EditarAsync(RutaEntregaDto entidad);
        Task CambiarEstadoAsync(int id, string estado);
    }
}
