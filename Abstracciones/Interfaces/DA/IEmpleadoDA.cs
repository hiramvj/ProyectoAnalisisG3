using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.DA
{
    public interface IEmpleadoDA
    {
        Task<List<EmpleadoDto>> ObtenerTodosAsync(bool activo);

        Task<EmpleadoDto?> ObtenerPorIdAsync(int id);

        Task AgregarAsync(EmpleadoDto dto);

        Task EditarAsync(EmpleadoDto dto);

        Task CambiarEstadoAsync(int id, bool activo);
    }
}
