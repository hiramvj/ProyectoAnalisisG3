using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface ITransportistaFlujo
    {
        Task<List<TransportistaDto>> ObtenerTodosAsync(bool activo);
        Task<TransportistaDto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(TransportistaDto dto);
        Task EditarAsync(TransportistaDto dto);
        Task CambiarEstadoAsync(int id, bool activo);
    }
}
