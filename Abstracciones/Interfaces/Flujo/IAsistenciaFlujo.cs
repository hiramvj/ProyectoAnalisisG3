using Abstracciones.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstracciones.Interfaces.Flujo
{
    public interface IAsistenciaFlujo
    {
        Task<List<AsistenciaDto>> ObtenerTodasAsync();

        Task<List<AsistenciaDto>> ObtenerPorEmpleadoAsync(int empleadoId);

        Task AgregarAsync(AsistenciaDto dto);
    }
}