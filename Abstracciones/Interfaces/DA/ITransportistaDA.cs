using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface ITransportistaDA
    {
        Task<List<TransportistaDto>> ObtenerTodosAsync(bool activo);
        Task<TransportistaDto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(TransportistaDto entidad);
        Task EditarAsync(TransportistaDto entidad);
        Task CambiarEstadoAsync(int id, bool activo);
    }
}