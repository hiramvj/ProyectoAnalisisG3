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
    public class AsistenciaFlujo : IAsistenciaFlujo
    {
        private readonly IAsistenciaDA _da;

        public AsistenciaFlujo(IAsistenciaDA da)
        {
            _da = da;
        }

        public async Task<List<AsistenciaDto>> ObtenerTodasAsync()
        {
            return await _da.ObtenerTodasAsync();
        }

        public async Task<List<AsistenciaDto>> ObtenerPorEmpleadoAsync(int empleadoId)
        {
            return await _da.ObtenerPorEmpleadoAsync(empleadoId);
        }

        public async Task AgregarAsync(AsistenciaDto dto)
        {
            await _da.AgregarAsync(dto);
        }
    }
}